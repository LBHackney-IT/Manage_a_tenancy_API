﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LBH.Utils;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services.Housing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class ETRAMeetingsAction : IETRAMeetingsAction
    {

        private HttpClient _client;
        private readonly AppConfiguration _configuration;
        private readonly HackneyHousingAPICallBuilder _apiCallBuilder;
        private ILoggerAdapter<ETRAMeetingsAction> _logger;
        private IHackneyHousingAPICall _ManageATenancyAPI;
        private IHackneyHousingAPICallBuilder _hackneyAccountApiBuilder;
        private IHackneyGetCRM365Token _crmAccessToken;

        public ETRAMeetingsAction(ILoggerAdapter<ETRAMeetingsAction> logger, IHackneyHousingAPICallBuilder apiCallBuilder, IHackneyHousingAPICall apiCall, IHackneyGetCRM365Token accessToken, IOptions<AppConfiguration> config)
        {
            _client = new HttpClient();
            _hackneyAccountApiBuilder = apiCallBuilder;
            _ManageATenancyAPI = apiCall;
            _logger = logger;
            _crmAccessToken = accessToken;
            _configuration = config?.Value;

        }
        public async Task<HackneyResult<JObject>> CreateETRAMeeting(ETRAIssue meetingInfo)
        {
            _logger.LogInformation($"Create Service Request");

            //set up client
            var token = _crmAccessToken.getCRM365AccessToken().Result;
            _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;
            _client.DefaultRequestHeaders.Add("Prefer", "return=representation");
            _client.DefaultRequestHeaders.Add("Prefer",
                "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");

            var sr = new JObject();
            var incidentid = string.Empty;
            var ticketnumber = string.Empty;
            //create the service request / incident
            if (meetingInfo.ServiceRequest != null)
            {
                if (!string.IsNullOrEmpty(meetingInfo.ServiceRequest.Subject))
                {
                    sr["subjectid@odata.bind"] = "/subjects(" + meetingInfo.ServiceRequest.Subject + ")";
                }
                //change this with dynamic account value 
                sr["customerid_account@odata.bind"] = "/accounts("+ _configuration.ETRAAccount +")";
                if (!string.IsNullOrEmpty(meetingInfo.ServiceRequest.Title))
                {
                    sr.Add("title", meetingInfo.ServiceRequest.Title);
                }
                if (!string.IsNullOrEmpty(meetingInfo.ServiceRequest.Description))
                {
                    sr.Add("description", meetingInfo.ServiceRequest.Description);
                }
                try
                {
                    var incidentquery = HousingAPIQueryBuilder.PostIncidentQuery();
                    var createResponseIncident =
                        await _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Post, incidentquery, sr);
                    if (createResponseIncident.StatusCode == HttpStatusCode.Created)
                    {
                        JObject createdServiceRequest = JsonConvert.DeserializeObject<JObject>(
                            await createResponseIncident.Content.ReadAsStringAsync());
                        incidentid = createdServiceRequest["incidentid"].ToString();
                        ticketnumber = createdServiceRequest["ticketnumber"].ToString();
                        //set servicerequestid/incidentid
                        meetingInfo.ServiceRequest.Id = incidentid;
                    }
                    if (!createResponseIncident.IsSuccessStatusCode)
                    {
                        throw new ServiceRequestException();
                    }
                    if (!string.IsNullOrWhiteSpace(meetingInfo.ServiceRequest.Description))
                    {
                        var annotationResult = await CreateAnnotation(meetingInfo.ServiceRequest.Description, meetingInfo.estateOfficerName,
                            meetingInfo.ServiceRequest.Id, _client);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (Utils.NullToString(incidentid) != "" && Utils.NullToString(ticketnumber) != "")
            {
                var tmiJObject = new JObject();

                //incidentid
                if (Utils.NullToString(incidentid) != "")
                {
                    tmiJObject["hackney_incidentId@odata.bind"] = "/incidents(" + incidentid + ")";
                }
                //Ticket number
                if (Utils.NullToString(ticketnumber) != "")
                {
                    tmiJObject.Add("hackney_name", ticketnumber);
                }
                // Parent Interaction (This is self referencing with TM Interactions when we need to create an issue, which needs to link to a parent ETRA meeting
                if (!string.IsNullOrEmpty(meetingInfo.parentInteractionId))
                {
                    tmiJObject.Add("hackney_parent_interactionid@odata.bind", " /hackney_tenancymanagementinteractionses(" + meetingInfo.parentInteractionId + ")");
                }
                if (Utils.NullToString(meetingInfo.subject) != "")
                {
                    tmiJObject["hackney_subjectId@odata.bind"] = "/subjects(" + meetingInfo.subject + ")";
                }
                if (Utils.NullToString(meetingInfo.estateOfficerId) != "")
                {
                    tmiJObject["hackney_estateofficer_createdbyid@odata.bind"] = "/hackney_estateofficers(" + meetingInfo.estateOfficerId + ")";
                }
                if (!string.IsNullOrEmpty(meetingInfo.officerPatchId))
                {
                    tmiJObject["hackney_estateofficerpatchid@odata.bind"] = "/hackney_estateofficerpatchs(" + meetingInfo.officerPatchId + ")";
                }
                if (!string.IsNullOrEmpty(meetingInfo.managerId))
                {
                    tmiJObject["hackney_managerpropertypatchid@odata.bind"] = "/hackney_estatemanagerareas(" + meetingInfo.managerId + ")";
                }
                if (!string.IsNullOrEmpty(meetingInfo.areaName))
                {
                    tmiJObject["hackney_areaname"] = meetingInfo.areaName;
                }
                    //add nature of enquiry
                tmiJObject.Add("hackney_natureofenquiry", meetingInfo.natureOfEnquiry);
                //add subject
                tmiJObject.Add("hackney_enquirysubject", meetingInfo.enquirySubject);
                // Process Type :- 0 Interaction , 1 TM Process , 2 Post Visit Action, 3 ETRA meeting issue
                if (meetingInfo.processType == "3")
                {
                    tmiJObject.Add("hackney_issuelocation", meetingInfo.issueLocation);
                }

                tmiJObject.Add("hackney_processtype", meetingInfo.processType);
                tmiJObject.Add("hackney_traid", meetingInfo.TRAId);

                try
                {
                    _logger.LogInformation($"Create Tenancy Management Interaction");
                    var incidentquery = HousingAPIQueryBuilder.PostETRAMeetingQuery();
                    var createResponseInteraction = await _ManageATenancyAPI.postHousingAPI(_client,incidentquery, tmiJObject);

                    if (createResponseInteraction != null)
                    {
                        if (!createResponseInteraction.IsSuccessStatusCode)
                        {
                            _logger.LogError($" Tenancy Management Interaction could not be created for ticket {ticketnumber} ");
                            throw new TenancyInteractionServiceException();
                        }
                        JObject apiResponse =
                            JsonConvert.DeserializeObject<JObject>(await createResponseInteraction.Content.ReadAsStringAsync());

                        JObject response = new JObject();
                        response.Add("interactionid", apiResponse["hackney_tenancymanagementinteractionsid"]);
                        response.Add("ticketnumber", ticketnumber);

                        return HackneyResult<JObject>.Create(response);
                    }
                    else
                    {
                        _logger.LogError($" Tenancy Management Interaction could not be created for ticket {ticketnumber} ");
                        throw new MissingTenancyInteractionRequestException();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else
            {
                _logger.LogError($" Service Request could not be created");
                throw new MissingTenancyInteractionRequestException();
            }
        }

        private async Task<object> CreateAnnotation(string notes, string estateOfficer, string serviceRequestId,
            HttpClient client)
        {
            try
            {
                string descriptionText = notes + " logged on  " + DateTime.Now.ToString() + " by  " + estateOfficer;
                HttpResponseMessage response;
                string annotationId = string.Empty;
                JObject note = new JObject();
                note["notetext"] = descriptionText;
                note["objectid_incident@odata.bind"] = "/incidents(" + serviceRequestId + ")";
                string requestUrl = "api/data/v8.2/annotations?$select=annotationid";

                response = await _ManageATenancyAPI.SendAsJsonAsync(client, HttpMethod.Post, requestUrl, note);
                if (response == null)
                {
                    _logger.LogError($" Response is null  {serviceRequestId}");
                    throw new NullResponseException();
                }
                else if (response.StatusCode == HttpStatusCode.Created) //201
                {
                    //Body should contain the requested annotation information.
                    JObject createdannotation = JsonConvert.DeserializeObject<JObject>(
                        await response.Content.ReadAsStringAsync());
                    //Because 'OData-EntityId' header not returned in a 201 response, you must instead 
                    // construct the URI.
                    annotationId = createdannotation["annotationid"].ToString();
                    return annotationId;
                }
                else
                {
                    _logger.LogError($" Response is null  {serviceRequestId}");
                    throw new TenancyServiceException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" Create Annotation Error with service reqest  {serviceRequestId}");
                throw ex;
            }

        }

        public async Task<object> GetETRAIssuesByTRAorETRAMeeting(string id, bool retrieveIssuesPerMeeting)
            {
                HttpResponseMessage result = null;
                try
                {

                    _logger.LogInformation($"Getting ETRA issues");
                    var token = _crmAccessToken.getCRM365AccessToken().Result;
                    _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;
                    _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");

                    var query = HousingAPIQueryBuilder.getETRAIssues(id, retrieveIssuesPerMeeting);

                    result = _ManageATenancyAPI.getHousingAPIResponse(_client, query, null).Result;
                    if (result != null)
                    {
                        if (!result.IsSuccessStatusCode)
                        {
                            throw new TenancyServiceException();
                        }

                        var issuesRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                        if (issuesRetrieveResponse?["value"] != null && issuesRetrieveResponse?["value"].Count() > 0)
                        {

                            List<JToken> issuesRetrievedList = issuesRetrieveResponse["value"].ToList();

                            return new
                            {
                                results = prepareIssuesResultObject(issuesRetrievedList)
                            };
                        }
                        else
                        {
                            return new
                            {
                                result = new object()
                            };
                        }
                    }
                    else
                    {
                        _logger.LogError($" ETRA issues missing for {id} ");
                        throw new NullResponseException();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Get ETRA issues Error " + ex.Message);
                    throw ex;

                }
            }

        private List<dynamic> prepareIssuesResultObject(List<JToken> responseList)
        {
            var groupIncident = (from response in responseList
                                 group response by new
                                 {
                                     interactionId = response["hackney_tenancymanagementinteractionsid"],
                                     isTransferred = response["hackney_transferred"],
                                     ticketNumber = response["hackney_name"],
                                     stateCode = response["statecode"],
                                     processStage = response["hackney_process_stage"],
                                     nccOfficersId = response["_hackney_estateofficer_createdbyid_value"],
                                     nccEstateOfficer = response["_hackney_estateofficer_createdbyid_value@OData.Community.Display.V1.FormattedValue"],
                                     createdon = response["createdon"],
                                     nccOfficerUpdatedById = response["_hackney_estateofficer_updatedbyid_value"],
                                     nccOfficerUpdatedByName = response["_hackney_estateofficer_updatedbyid_value@OData.Community.Display.V1.FormattedValue"],
                                     natureOfEnquiryId = response["hackney_natureofenquiry"],
                                     natureOfEnquiry = response["hackney_natureofenquiry@OData.Community.Display.V1.FormattedValue"],
                                     enquirySubjectId = response["hackney_enquirysubject"],
                                     enquirysubject = response["hackney_enquirysubject@OData.Community.Display.V1.FormattedValue"],
                                     incidentId = response["_hackney_incidentid_value"],
                                     areamanagerId = response["_hackney_managerpropertypatchid_value"],
                                     areaManagerName = response["ManagerFirstName"] + " " + response["ManagerLastName"],
                                     officerPatchId = response["_hackney_estateofficerpatchid_value"],
                                     officerPatchName = response["OfficerFirstName"] + " " + response["OfficerLastName"],
                                     areaName = response["hackney_areaname@OData.Community.Display.V1.FormattedValue"]
                                    

                                 } into grp
                                 select new
                                 {
                                     grp.Key.incidentId,
                                     grp.Key.isTransferred,
                                     grp.Key.ticketNumber,
                                     grp.Key.stateCode,
                                     grp.Key.processStage,
                                     grp.Key.nccOfficersId,
                                     grp.Key.nccEstateOfficer,
                                     grp.Key.createdon,
                                     grp.Key.nccOfficerUpdatedById,
                                     grp.Key.nccOfficerUpdatedByName,
                                     grp.Key.natureOfEnquiryId,
                                     grp.Key.natureOfEnquiry,
                                     grp.Key.enquirySubjectId,
                                     grp.Key.enquirysubject,
                                     grp.Key.interactionId,
                                     grp.Key.areamanagerId,
                                     grp.Key.areaManagerName,
                                     grp.Key.officerPatchId,
                                     grp.Key.officerPatchName,
                                     grp.Key.areaName,
                                     Annotation = grp.ToList()

                                 });

            var tenancyList = new List<dynamic>();

            foreach (dynamic response in groupIncident)
            {
                dynamic tenancyObj = new ExpandoObject();
                tenancyObj.incidentId = response.incidentId;
                tenancyObj.isTransferred = response.isTransferred;
                tenancyObj.ticketNumber = response.ticketNumber;
                tenancyObj.stateCode = response.stateCode;
                tenancyObj.processStage = response.processStage;
                tenancyObj.nccOfficersId = response.nccOfficersId;
                tenancyObj.nccEstateOfficer = response.nccEstateOfficer;
                tenancyObj.createdon = response.createdon.ToString("yyyy-MM-dd HH:mm:ss");
                tenancyObj.nccOfficerUpdatedById = response.nccOfficerUpdatedById;
                tenancyObj.nccOfficerUpdatedByName = response.nccOfficerUpdatedByName;
                tenancyObj.natureOfEnquiryId = response.natureOfEnquiryId;
                tenancyObj.natureOfEnquiry = response.natureOfEnquiry;
                tenancyObj.enquirySubjectId = response.enquirySubjectId;
                tenancyObj.enquirysubject = response.enquirysubject;
                tenancyObj.interactionId = response.interactionId;
                tenancyObj.areamanagerId = response.areamanagerId;
                tenancyObj.areaManagerName = response.areaManagerName;
                tenancyObj.officerPatchId = response.officerPatchId;
                tenancyObj.officerPatchName = response.officerPatchName;
                tenancyObj.areaName = response.areaName;
                tenancyObj.AnnotationList = new List<ExpandoObject>();

                foreach (var annotationResponse in response.Annotation)
                {
                    dynamic annotation = new ExpandoObject();
                    annotation.noteText = annotationResponse["annotation2_x002e_notetext"] != null ? annotationResponse["annotation2_x002e_notetext"] : string.Empty;
                    annotation.annotationId = annotationResponse["annotation2_x002e_annotationid"] != null ? annotationResponse["annotation2_x002e_annotationid"] : string.Empty;
                    annotation.noteCreatedOn = annotationResponse["annotation2_x002e_createdon"] != null ? Utils.ConverDateTimeToLocal(annotationResponse["annotation2_x002e_createdon"].ToString()) : string.Empty;
                    tenancyObj.AnnotationList.Add(annotation);
                }
                tenancyList.Add(tenancyObj);
            }
            return tenancyList;
        }

    }
}
