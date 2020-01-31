﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LBH.Utils;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
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
        private readonly IDateService _dateService;

        public ETRAMeetingsAction(ILoggerAdapter<ETRAMeetingsAction> logger, IHackneyHousingAPICallBuilder apiCallBuilder, IHackneyHousingAPICall apiCall, IHackneyGetCRM365Token accessToken, IOptions<AppConfiguration> config, IDateService dateService)
        {
            //set up client
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Prefer", "return=representation");
            _client.DefaultRequestHeaders.Add("Prefer",
                "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
            _hackneyAccountApiBuilder = apiCallBuilder;
            _ManageATenancyAPI = apiCall;
            _logger = logger;
            _crmAccessToken = accessToken;
            _configuration = config?.Value;
            _dateService = dateService;

        }
        public async Task<CreateETRAMeetingActionResponse> CreateETRAMeeting(ETRAIssue meetingInfo)
        {
            _logger.LogInformation($"Create Service Request");

            //set up client
            var token = _crmAccessToken.getCRM365AccessToken().Result;
            _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;

            var sr = new JObject();
            var incidentid = string.Empty;
            var ticketnumber = string.Empty;
            string annotationResult = string.Empty;
            //create the service request / incident
            if (meetingInfo.ServiceRequest != null)
            {
                if (!string.IsNullOrEmpty(_configuration.SubjectId))
                {
                    sr["subjectid@odata.bind"] = "/subjects(" + _configuration.SubjectId + ")";
                }
                //change this with dynamic account value 
                sr["customerid_account@odata.bind"] = "/accounts(" + _configuration.ETRAAccount + ")";
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
                    _client.DefaultRequestHeaders.Add("Prefer", "return=representation");


                    ///
                    ///
                    ///
                    /// SUBMIT SERVICE REQUEST
                    ///
                    ///
                    ///
                    ///
                    var createResponseIncident =
                        await _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Post, incidentquery, sr);


                    if (createResponseIncident.IsSuccessStatusCode)
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
                        annotationResult = await CreateAnnotation(meetingInfo.ServiceRequest.Description, meetingInfo.estateOfficerName,
                           meetingInfo.ServiceRequest.Id, null);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (Utils.NullToString(incidentid) != "" && Utils.NullToString(ticketnumber) != "")
            {
                JObject tmiJObject = await CreateTenancyManagementInteractionJObject(meetingInfo, incidentid, ticketnumber);

                try
                {
                    _logger.LogInformation($"Create Tenancy Management Interaction");
                    var incidentquery = HousingAPIQueryBuilder.PostETRAMeetingQuery();
                    var createResponseInteraction = await _ManageATenancyAPI.postHousingAPI(_client, incidentquery, tmiJObject);

                    if (createResponseInteraction != null)
                    {
                        if (!createResponseInteraction.IsSuccessStatusCode)
                        {
                            _logger.LogError($" Tenancy Management Interaction could not be created for ticket {ticketnumber} ");
                            throw new TenancyInteractionServiceException();
                        }
                        JObject apiResponse =
                            JsonConvert.DeserializeObject<JObject>(await createResponseInteraction.Content.ReadAsStringAsync());

                        var response = new CreateETRAMeetingActionResponse
                        {
                            IncidentId = Guid.Parse(incidentid),
                            InteractionId = Guid.Parse(apiResponse["hackney_tenancymanagementinteractionsid"].ToString()),
                            TicketNumber = ticketnumber,
                            AnnotationId = annotationResult
                        };

                        return response;
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

        private async Task<JObject> CreateTenancyManagementInteractionJObject(ETRAIssue meetingInfo, string incidentid, string ticketnumber)
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
            if (Utils.NullToString(_configuration.SubjectId) != "")
            {
                tmiJObject["hackney_subjectId@odata.bind"] = "/subjects(" + _configuration.SubjectId + ")";
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
                var dueDate = await _dateService.GetIssueResponseDueDate(DateTime.Now, 3);
                tmiJObject.Add("hackney_issuedeadlinedate", dueDate);
            }

            tmiJObject.Add("hackney_processtype", meetingInfo.processType);
            tmiJObject.Add("hackney_process_stage", (int)HackneyProcessStage.NotCompleted);
            tmiJObject.Add("hackney_traid", meetingInfo.TRAId);
            return tmiJObject;
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
                _logger.LogError($"GetAsync ETRA issues Error " + ex.Message);
                throw ex;

            }
        }

        public async Task<IList<MeetingIssueOutputModel>> GetETRAIssuesForMeeting(Guid id, CancellationToken cancellationToken)
        {
            HttpResponseMessage result = null;
            try
            {

                _logger.LogInformation($"Getting ETRA issues");
                var token = _crmAccessToken.getCRM365AccessToken().Result;
                _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");

                var query = HousingAPIQueryBuilder.getETRAIssues(id.ToString(), true);

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

                        IList<MeetingIssueOutputModel> list = null;
                        list = (from dataresponse in issuesRetrievedList
                                group dataresponse by new
                                {
                                    Id = dataresponse["hackney_tenancymanagementinteractionsid"],
                                    Location = dataresponse["hackney_issuelocation"],
                                    IssueId = dataresponse["hackney_enquirysubject"],
                                    ServiceRequestId = dataresponse["_hackney_incidentid_value"],
                                    AreaId = dataresponse["hackney_areaname"],
                                    DueDate = dataresponse["hackney_issuedeadlinedate"]
                                } into grp
                                select new
                                {
                                    item = grp.Key,
                                    annotations = grp.ToList().Select(si => si["annotation2_x002e_notetext"])


                                }).ToList().Select(x =>
                                new MeetingIssueOutputModel
                                {
                                    Id = x.item.Id.ToObject<Guid>(),
                                    Location = new Location
                                    {
                                        Name = x.item.Location?.ToString()
                                    },
                                    IssueType = new IssueType
                                    {
                                        IssueId = x.item.IssueId?.ToString()
                                    },
                                    Notes = string.Join(Environment.NewLine, x.annotations)
                                }).ToList();

                        return list;
                    }

                    return null;
                }
                else
                {
                    _logger.LogError($" ETRA issues missing for {id} ");
                    throw new NullResponseException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAsync ETRA issues Error " + ex.Message);
                throw ex;

            }
        }

        public async Task<ETRAUpdateResponse> UpdateIssue(UpdateETRAIssue issueToBeUpdated)
        {
            try
            {
                _logger.LogInformation($"Update for issue with id {issueToBeUpdated.issueInteractionId} is starting");
                var token = await _crmAccessToken.getCRM365AccessToken();
                _client = await _hackneyAccountApiBuilder.CreateRequest(token);
                var result = new ETRAUpdateResponse
                {
                    InteractionId = issueToBeUpdated.issueInteractionId,
                    IncidentId = issueToBeUpdated.issueIncidentId
                };
                //delete issue scenario
                if (issueToBeUpdated.issueIsToBeDeleted)
                {
                    await CloseIncidentAndDeleteIssue(issueToBeUpdated.note,
                        issueToBeUpdated.issueIncidentId, issueToBeUpdated.issueInteractionId);

                    result.Action = "Deleted";
                    return result;
                }

                var issueUpdateObject = new JObject();

                //update service area scenario
                if (issueToBeUpdated.serviceArea != null)
                {
                    issueUpdateObject.Add("hackney_servicearea", issueToBeUpdated.serviceArea);
                }

                //update issue stage (status)
                if (issueToBeUpdated.issueStage != null)
                {
                    issueUpdateObject.Add("hackney_process_stage", issueToBeUpdated.issueStage);
                }

                if (!string.IsNullOrEmpty(issueToBeUpdated.EnquirySubject))
                    issueUpdateObject.Add("hackney_enquirysubject", issueToBeUpdated.EnquirySubject);

                if (!string.IsNullOrEmpty(issueToBeUpdated.IssueLocation))
                    issueUpdateObject.Add("hackney_issuelocation", issueToBeUpdated.IssueLocation);

                if (issueToBeUpdated.PDFId.HasValue && issueToBeUpdated.PDFId.Value != Guid.Empty)
                    issueUpdateObject.Add("hackney_pdfreference", issueToBeUpdated.PDFId.Value);

                if (issueToBeUpdated.SignatureId.HasValue && issueToBeUpdated.SignatureId.Value != Guid.Empty)
                    issueUpdateObject.Add("hackney_signaturereference", issueToBeUpdated.SignatureId.Value);

                issueUpdateObject.Add("hackney_estateofficer_updatedbyid@odata.bind",
                    $"/hackney_estateofficers({issueToBeUpdated.estateOfficerId})");

                //adding new notes scenario (could be when issue is updated by officer or when response is added by service
                if (issueToBeUpdated.isNewNote)
                {
                    var subjId = issueToBeUpdated.AnnotationSubjectId.HasValue
                        ? issueToBeUpdated.AnnotationSubjectId.Value.ToString()
                        : null;
                    if (!string.IsNullOrEmpty(issueToBeUpdated.note))
                    {
                        var annotationid = await CreateAnnotation(issueToBeUpdated.note, issueToBeUpdated.estateOfficerName,
                            issueToBeUpdated.issueIncidentId.ToString(), subjId);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(issueToBeUpdated.note))
                    {
                        await UpdateAnnotation(issueToBeUpdated.note, issueToBeUpdated.estateOfficerName,
                        issueToBeUpdated.annotationId.ToString());
                    }
                }

                //updated interaction record
                var updateIssueIntractionQuery = HousingAPIQueryBuilder.updateIssueQuery(issueToBeUpdated.issueInteractionId.ToString());

                var updateIntractionResponse =
                _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Patch, updateIssueIntractionQuery, issueUpdateObject).Result;
                if (!updateIntractionResponse.IsSuccessStatusCode)
                {
                    throw new TenancyServiceException();
                }

                result.Action = "Updated";
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<ETRAUpdateResponse> AddETRAIssueResponse(string id, ETRAIssueResponseRequest request)
        {
            var token = await _crmAccessToken.getCRM365AccessToken();
            _client = await _hackneyAccountApiBuilder.CreateRequest(token);

            var issueUpdateObject = new JObject();

            issueUpdateObject.Add("hackney_servicearea", request.ServiceAreaId);
            issueUpdateObject.Add("hackney_process_stage", (int)request.IssueStage);

            if (request.ProjectedCompletionDate.HasValue)
            {
                issueUpdateObject.Add("hackney_completiondate", request.ProjectedCompletionDate);
            }

            var completionDateText = string.Empty;

            if (request.ProjectedCompletionDate.HasValue && (int)request.IssueStage == 0)
            {
                completionDateText = $"Not yet completed by service area. Projected completion date: {request.ProjectedCompletionDate.Value.ToString("dddd dd MMMM yyyy")}" + Environment.NewLine;
            }
            else
                completionDateText = $"Completed by service area " + Environment.NewLine;

            var noteText = $"Response from: {request.ServiceAreaName}" + Environment.NewLine + request.ResponseText + Environment.NewLine + completionDateText + $"Responder: {request.ResponderName} on {DateTime.Now}";

            var annotationId = await CreateAnnotationAsync(noteText, request.IssueIncidentId.ToString(), request.AnnotationSubjectId.ToString());

            var updateIssueIntractionQuery = HousingAPIQueryBuilder.updateIssueQuery(id);

            var updateIntractionResponse = await _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Patch, updateIssueIntractionQuery, issueUpdateObject);

            if (!updateIntractionResponse.IsSuccessStatusCode)
            {
                throw new TenancyServiceException();
            }

            var result = new ETRAUpdateResponse
            {
                Action = "Updated",
                IncidentId = request.IssueIncidentId,
                InteractionId = Guid.Parse(id),
                AnnotationId = Guid.Parse(annotationId)
            };

            return result;
        }

        public async Task<ETRAUpdateResponse> RejectETRAIssueResponse(string id, ETRAIssueRejectResponseRequest request)
        {
            var token = await _crmAccessToken.getCRM365AccessToken();
            _client = await _hackneyAccountApiBuilder.CreateRequest(token);

            //set the process stage to null, i.e. awaiting response
            var issueUpdateObject = new JObject
            {
                { "hackney_process_stage", null }
            };

            var noteText = $"Response rejected {Environment.NewLine + request.ResponseText}Responder: {request.ResponderName} on {DateTime.Now}";

            var annotationId = await CreateAnnotationAsync(noteText, request.IssueIncidentId.ToString(), request.AnnotationSubjectId.ToString());

            var updateIssueIntractionQuery = HousingAPIQueryBuilder.updateIssueQuery(id);

            var updateIntractionResponse = await _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Patch, updateIssueIntractionQuery, issueUpdateObject);

            if (!updateIntractionResponse.IsSuccessStatusCode)
            {
                throw new TenancyServiceException();
            }

            var result = new ETRAUpdateResponse
            {
                Action = "Updated",
                IncidentId = request.IssueIncidentId,
                InteractionId = Guid.Parse(id),
                AnnotationId = Guid.Parse(annotationId)
            };

            return result;
        }

        public async Task<ETRAMeeting> GetMeetingAsync(string id)
        {
            var token = await _crmAccessToken.getCRM365AccessToken();
            _client = await _hackneyAccountApiBuilder.CreateRequest(token);
            var getMeetingQuery = HousingAPIQueryBuilder.GetActionById(id);

            var result = await _ManageATenancyAPI.getHousingAPIResponse(_client, getMeetingQuery, null);

            if (result != null)
            {
                if (!result.IsSuccessStatusCode)
                {
                    throw new TenancyServiceException();
                }

                var meeting = ETRAMeeting.Create(JsonConvert.DeserializeObject<dynamic>(await result.Content.ReadAsStringAsync()));

                return meeting;
            }
            else
            {
                _logger.LogError($"ETRA meeting missing for id: {id}");
                throw new NullResponseException();
            }
        }

        public async Task<GetEtraMeetingOutputModel> GetMeetingV2Async(Guid id, CancellationToken cancellationToken)
        {
            var token = await _crmAccessToken.getCRM365AccessToken();
            _client = await _hackneyAccountApiBuilder.CreateRequest(token);
            _client.DefaultRequestHeaders.Add("Prefer",
                "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
            var getMeetingQuery = HousingAPIQueryBuilder.getEtraMeetingV2(id);

            var result = await _ManageATenancyAPI.getHousingAPIResponse(_client, getMeetingQuery, null);

            if (result != null)
            {
                if (!result.IsSuccessStatusCode)
                {
                    throw new TenancyServiceException();
                }

                JObject response = JsonConvert.DeserializeObject<JObject>(await result.Content.ReadAsStringAsync());

                var crmMeeting = response["value"];
                crmMeeting = crmMeeting[0];

                var interactionId = crmMeeting["hackney_tenancymanagementinteractionsid"];

                var Councillors = (string)crmMeeting["hackney_councillorsattendingmeeting"];
                var HackneyStaff = (string)crmMeeting["hackney_othercouncilstaffattendingmeeting"];
                int Attendees = 1;
                if (!string.IsNullOrEmpty((string)crmMeeting["hackney_totalmeetingattendees"]))
                {
                    Attendees = (int)crmMeeting["hackney_totalmeetingattendees"];
                }


                //var Attendees = (int)crmMeeting["hackney_totalmeetingattendees"];

                var attendees = new MeetingAttendees
                {
                    Councillors = Councillors,
                    HackneyStaff = HackneyStaff,
                    Attendees = Attendees
                };

                var signOffDate = crmMeeting["hackney_confirmationdate"];

                SignOff signOff = null;
                if (signOffDate != null)
                {
                    signOff = new SignOff
                    {
                        SignatureId = crmMeeting["hackney_signaturereference"] != null ? crmMeeting["hackney_signaturereference"].ToObject<Guid>() : Guid.Empty,
                        SignOffDate = signOffDate.ToObject<DateTime>(),
                        Role = crmMeeting["hackney_signatoryrole"].ToString(),
                        Name = crmMeeting["hackney_signatoryname"].ToString(),
                    };

                }

                var name = crmMeeting["incident1_x002e_description"].ToString();
                var createdOn = crmMeeting["createdon"].ToObject<DateTime>();

                var outputModel = new GetEtraMeetingOutputModel
                {
                    Id = id,
                    MeetingName = name,
                    MeetingAttendance = attendees,
                    CreatedOn = createdOn,
                    SignOff = signOff,
                    IsSignedOff = signOff != null ? true : false,


                };

                return outputModel;
            }
            else
            {
                _logger.LogError($"ETRA meeting missing for id: {id}");
                throw new NullResponseException();
            }
        }


        public async Task<IEnumerable<ETRAMeeting>> GetETRAMeetingsForTRAId(string id)
        {
            var query = HousingAPIQueryBuilder.GetETRAMeetingsByTRAId(id);
            var token = await _crmAccessToken.getCRM365AccessToken();
            _client = await _hackneyAccountApiBuilder.CreateRequest(token);

            var meetingsResponse = await _ManageATenancyAPI.getHousingAPIResponse(_client, query, null);

            if (meetingsResponse == null)
                throw new NullResponseException();

            if (!meetingsResponse.IsSuccessStatusCode)
                throw new TenancyServiceException();

            var responseObject = JsonConvert.DeserializeObject<JObject>(await meetingsResponse.Content.ReadAsStringAsync());

            if (responseObject?["value"] == null || !responseObject["value"].Any())
                return new List<ETRAMeeting>();

            var meetings = responseObject["value"].ToList();
            var result = meetings.Select(x => ETRAMeeting.Create(x));
            return result;
        }

        public async Task<RecordETRAMeetingAttendanceResponse> RecordETRAMeetingAttendance(string id, RecordETRAMeetingAttendanceRequest request)
        {
            var attendance = new JObject
            {
                { "hackney_councillorsattendingmeeting", request.Councillors },
                { "hackney_othercouncilstaffattendingmeeting", request.OtherCouncilStaff },
                { "hackney_totalmeetingattendees", request.TotalAttendees }
            };

            var updateIssueIntractionQuery = HousingAPIQueryBuilder.updateIssueQuery(id);
            var token = await _crmAccessToken.getCRM365AccessToken();
            _client = await _hackneyAccountApiBuilder.CreateRequest(token);

            var updateIntractionResponse = await
                _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Patch, updateIssueIntractionQuery, attendance);

            if (!updateIntractionResponse.IsSuccessStatusCode)
            {
                throw new TenancyServiceException();
            }

            return new RecordETRAMeetingAttendanceResponse
            {
                TotalAttendees = request.TotalAttendees,
                OtherCouncilStaff = request.OtherCouncilStaff,
                Councillors = request.Councillors,
                MeetingId = id,
                Recorded = updateIntractionResponse.IsSuccessStatusCode
            };
        }

        public async Task<FinaliseETRAMeetingResponse> FinaliseMeeting(string id, FinaliseETRAMeetingRequest request)
        {
            var signOffDate = DateTime.Now;
            var confirmation = new JObject {
                { "hackney_confirmationdate", signOffDate }
            };

            if (request != null)
            {
                if (!string.IsNullOrEmpty(request.Role))
                    confirmation.Add("hackney_signatoryrole", request.Role);

                if (!string.IsNullOrEmpty(request.Name))
                    confirmation.Add("hackney_signatoryname", request.Name);

                if (request.SignatureId != Guid.NewGuid())
                    confirmation.Add("hackney_signaturereference", request.SignatureId);
            }

            var updateIssueIntractionQuery = HousingAPIQueryBuilder.updateIssueQuery(id);
            var token = await _crmAccessToken.getCRM365AccessToken();
            _client = await _hackneyAccountApiBuilder.CreateRequest(token);

            var updateIntractionResponse = await
                _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Patch, updateIssueIntractionQuery, confirmation);

            if (!updateIntractionResponse.IsSuccessStatusCode)
            {
                throw new TenancyServiceException();
            }

            return new FinaliseETRAMeetingResponse { Id = id, IsFinalised = updateIntractionResponse.IsSuccessStatusCode, SignOffDate = signOffDate };
        }

        public async Task<IncidentClosedResponse> CloseIncident(string closingNotes, Guid incidentId)
        {
            string requestUrl = "/api/data/v8.2/CloseIncident";
            var srClose = new JObject
            {
                { "incidentid@odata.bind", $"/incidents({incidentId})" },
                { "description", closingNotes }
            };
            var resolution = new JObject
            {
                ["IncidentResolution"] = srClose,
                ["Status"] = _configuration.CompletedClosureType //closing incident
            };

            var response = await _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Post, requestUrl, resolution);

            if (!response.IsSuccessStatusCode)
                throw new TenancyServiceException();

            return new IncidentClosedResponse
            {
                IncidentId = incidentId,
                Status = "Closed"
            };
        }

        public async Task<GetAllEtraIssuesThatNeedEscalatingOutputModel> GetAllEtraIssuesThatNeedEscalatingAsync(DateTime fromDate, CancellationToken cancellationToken)
        {
            var token = await _crmAccessToken.getCRM365AccessToken();
            _client = await _hackneyAccountApiBuilder.CreateRequest(token);
            _client.DefaultRequestHeaders.Add("Prefer",
                "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
            var getMeetingQuery = HousingAPIQueryBuilder.getIssuesThatNeedEscalating(fromDate);

            var result = await _ManageATenancyAPI.getHousingAPIResponse(_client, getMeetingQuery, null);
            JObject response = JsonConvert.DeserializeObject<JObject>(await result.Content.ReadAsStringAsync());
            if (response?["value"] != null && response?["value"].Count() > 0)
            {
                List<JToken> issuesRetrievedList = response["value"].ToList();

                IList<EscalateMeetingIssueInputModel> list = null;

                list = (from dataresponse in issuesRetrievedList
                        group dataresponse by new
                        {
                            Id = dataresponse["hackney_tenancymanagementinteractionsid"],
                            Location = dataresponse["hackney_issuelocation"],
                            IssueId= dataresponse["hackney_enquirysubject"],
                            IssueName = dataresponse["hackney_enquirysubject@OData.Community.Display.V1.FormattedValue"],
                            ServiceRequestId = dataresponse["_hackney_incidentid_value"],
                            AreaId = dataresponse["hackney_areaname"],
                            DueDate = dataresponse["hackney_issuedeadlinedate"],
                            CreatedOn = dataresponse["createdon"],
                            TraId = dataresponse["hackney_traid"],
                            HousingOfficerName = dataresponse["OfficerFirstName"] + " " + dataresponse["OfficerLastName"],
                            TicketNumber= dataresponse["hackney_name"] 
                        } into grp
                        select new
                        {
                            item = grp.Key,
                            annotations = grp.ToList().Select(si => si["annotation4_x002e_notetext"])
                           


                        }).ToList().Select(x =>
                        new EscalateMeetingIssueInputModel
                        {
                            Id = x.item.Id.ToObject<Guid>(),
                            Location = new Location
                            {
                                Name = x.item.Location?.ToString()
                            },
                            IssueType = new IssueType
                            {
                                IssueId = x.item.IssueId?.ToString()
                            },
                            Notes = string.Join(Environment.NewLine, x.annotations),
                            // Notes = string.Join(",", grp.ToList().ForEac = x["annotation2_x002e_notetext"]?.ToString()),
                            AreaId = x.item.AreaId?.ToString(),
                            ServiceRequestId = x.item.ServiceRequestId.ToObject<Guid>(),
                            DueDate = x.item.DueDate?.ToObject<DateTime>().Date,
                            TraId=int.Parse(x.item.TraId.ToString()),
                            HousingOfficerName=x.item.HousingOfficerName,
                            CreatedOn=x.item.CreatedOn?.ToObject<DateTime>().Date,
                            TicketNumber=x.item.TicketNumber?.ToString(),
                            IssueName = x.item.IssueName?.ToString()
                        }).ToList();

                var outputModel = new GetAllEtraIssuesThatNeedEscalatingOutputModel
                {
                    IssuesThatNeedEscalating = list
                };
                return outputModel;
            }
            return null;
        }

        public async Task<bool> EscalateIssue(EscalateMeetingIssueInputModel issue, CancellationToken cancellationToken)
        {
            var signOffDate = DateTime.Now;
            var confirmation = new JObject {
                { "hackney_process_stage", (int)HackneyProcessStage.Escalated }
            };

            var updateIssueIntractionQuery = HousingAPIQueryBuilder.updateIssueQuery(issue.Id.ToString());
            var token = await _crmAccessToken.getCRM365AccessToken();
            _client = await _hackneyAccountApiBuilder.CreateRequest(token);

            var updateIntractionResponse = await _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Patch, updateIssueIntractionQuery, confirmation);

            if (!updateIntractionResponse.IsSuccessStatusCode)
            {
                return false;
            }

            await CreateAnnotationAsync($"This issue was automatically escalated on {DateTime.Now.ToString("R")} because a response wasn't issued within 15 working days.", issue.ServiceRequestId.ToString(), null);

            return true;
        }

        public async Task<bool> CloseMeetingInteraction(CloseETRAMeetingRequest closeETRAMeetingRequests)
        {
            string interactionQuery = HousingAPIQueryBuilder.updateInteractionQuery(closeETRAMeetingRequests.InteractionId.ToString());

            HttpResponseMessage updateResponse = new HttpResponseMessage();

            _logger.LogInformation($"Update Tenancy Management Interaction");
            _logger.LogInformation($"Update description which is the notes that is sent from the UI ,Name of the Asset officer who has updated and the current time stamp");
            var token = _crmAccessToken.getCRM365AccessToken().Result;
            _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;


            JObject tenancyInteraction = new JObject();
            _logger.LogError($"StateCodeInactive {Constants.StateCodeInactive}");
            _logger.LogError($"StatusCodeInActive {Constants.StatusCodeInActive}");

            tenancyInteraction.Add("statuscode", Constants.StatusCodeInActive);
            tenancyInteraction.Add("statecode", Constants.StateCodeInactive);
            tenancyInteraction.Add("hackney_process_stage", (int)closeETRAMeetingRequests.MeetingStage);


            tenancyInteraction["hackney_estateofficer_updatedbyid@odata.bind"] = "/hackney_estateofficers(" + closeETRAMeetingRequests.UpdatedByOfficerId + ")";
            tenancyInteraction.Add("modifiedon", DateTime.Now);

            bool returnResponse = await _ManageATenancyAPI.UpdateObject(_client, interactionQuery, tenancyInteraction);

            return returnResponse;

        }
        #region Private methods

        private async Task UpdateAnnotation(string notes, string estateOfficer, string annotationId)
        {
            var descriptionText = $"{notes} logged on {DateTime.Now} by {estateOfficer}";
            await UpdateAnnotation(descriptionText, annotationId);
        }

        private async Task UpdateAnnotation(string notes, string annotationId)
        {
            try
            {
                HttpResponseMessage response;
                JObject note = new JObject();
                note["notetext"] = notes;
                string requestUrl = $"api/data/v8.2/annotations({annotationId})";

                response = await _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Patch, requestUrl, note);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($" An error has occured while updating the annotation with id:  {annotationId}");
                    throw new TenancyServiceException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" Update Annotation Error {ex.Message} with id  {annotationId}");
                throw ex;
            }

        }

        private async Task CloseIncidentAndDeleteIssue(string closingNotes, Guid incidentId, Guid interactionId)
        {
            try
            {
                await CloseIncident(closingNotes, incidentId);

                string deleteUrl = $"/api/data/v8.2/hackney_tenancymanagementinteractionses({interactionId})";
                var deleteIneraction = await _ManageATenancyAPI.deleteObjectAPIResponse(_client, deleteUrl);
                if (!deleteIneraction.IsSuccessStatusCode)
                {
                    throw new TenancyServiceException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" Close incident error {ex.Message} with service request {incidentId}");
                throw new TenancyServiceException();
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
                                     nccOfficerName = response["_hackney_estateofficer_createdbyid_value@OData.Community.Display.V1.FormattedValue"],
                                     createdon = response["createdon"],
                                     deadlineDate = response["hackney_issuedeadlinedate"],
                                     parentInteractionId = response["_hackney_parent_interactionid_value"],
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
                                     areaName = response["hackney_areaname@OData.Community.Display.V1.FormattedValue"],
                                     traId = response["hackney_traid"],
                                     issueLocation = response["hackney_issuelocation"],
                                     processType = response["hackney_processtype"],
                                     serviceArea = response["hackney_servicearea"],
                                     serviceAreaName = response["hackney_servicearea@OData.Community.Display.V1.FormattedValue"]


                                 } into grp
                                 select new
                                 {
                                     grp.Key.incidentId,
                                     grp.Key.isTransferred,
                                     grp.Key.ticketNumber,
                                     grp.Key.stateCode,
                                     grp.Key.processStage,
                                     grp.Key.nccOfficersId,
                                     grp.Key.nccOfficerName,
                                     grp.Key.createdon,
                                     grp.Key.deadlineDate,
                                     grp.Key.parentInteractionId,
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
                                     grp.Key.traId,
                                     grp.Key.issueLocation,
                                     grp.Key.processType,
                                     grp.Key.serviceArea,
                                     grp.Key.serviceAreaName,
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
                tenancyObj.nccOfficerName = response.nccOfficerName;
                tenancyObj.createdon = response.createdon.ToString("yyyy-MM-dd HH:mm:ss");
                tenancyObj.deadlineDate = response.deadlineDate;
                tenancyObj.parentInteractionId = response.parentInteractionId;
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
                tenancyObj.traId = response.traId;
                tenancyObj.issueLocation = response.issueLocation;
                tenancyObj.processType = response.processType;
                tenancyObj.serviceArea = response.serviceArea;
                tenancyObj.serviceAreaName = response.serviceAreaName;
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

        private async Task<string> CreateAnnotation(string notes, string estateOfficer, string serviceRequestId, string annotationSubjectId)
        {
            string descriptionText = $"{notes}";
            return await CreateAnnotationAsync(descriptionText, serviceRequestId, annotationSubjectId);
        }

        private async Task<string> CreateAnnotationAsync(string notes, string serviceRequestId, string annotationSubjectId)
        {
            try
            {
                HttpResponseMessage response;
                string annotationId = string.Empty;
                JObject note = new JObject();
                note["notetext"] = notes;
                note["objectid_incident@odata.bind"] = "/incidents(" + serviceRequestId + ")";
                if (!string.IsNullOrEmpty(annotationSubjectId))
                    note["subject"] = annotationSubjectId;

                string requestUrl = "api/data/v8.2/annotations?$select=annotationid";
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                response = await _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Post, requestUrl, note);
                if (response == null)
                {
                    _logger.LogError($" Response is null  {serviceRequestId}");
                    throw new NullResponseException();
                }
                else if (response.IsSuccessStatusCode)
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
        #endregion
    }
}