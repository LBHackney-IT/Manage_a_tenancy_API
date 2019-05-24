using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services.Housing;
using LBH.Utils;
using ManageATenancyAPI.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class TenancyManagementActions
    {

        private HttpClient _client;
        private readonly AppConfiguration _configuration;
        private readonly HackneyHousingAPICallBuilder _apiCallBuilder;
        private ILoggerAdapter<TenancyManagementActions> _logger;
        private IHackneyHousingAPICall _ManageATenancyAPI;
        private IHackneyHousingAPICallBuilder _hackneyAccountApiBuilder;
        private IHackneyGetCRM365Token _crmAccessToken;

        public TenancyManagementActions(ILoggerAdapter<TenancyManagementActions> logger,  IHackneyHousingAPICallBuilder apiCallBuilder, IHackneyHousingAPICall apiCall, IHackneyGetCRM365Token accessToken, IOptions<AppConfiguration> config)
        {
            _client = new HttpClient();
            _hackneyAccountApiBuilder = apiCallBuilder;
            _ManageATenancyAPI = apiCall;
            _logger = logger;
            _crmAccessToken = accessToken;
            _configuration = config?.Value;

        }
        public async Task<TenancyManagement> CreateTenancyManagementInteraction(TenancyManagement interaction)
        {
            _logger.LogInformation($"Create Service Request");

            var sr = new JObject();
            var incidentid = string.Empty;
            var ticketnumber = string.Empty;

            if (interaction.ServiceRequest != null)
            {
                if (!string.IsNullOrEmpty(interaction.ServiceRequest.Subject))
                {
                    sr["subjectid@odata.bind"] = "/subjects(" + interaction.ServiceRequest.Subject + ")";
                }
                if (!string.IsNullOrEmpty(interaction.ServiceRequest.ContactId))
                {
                    sr["customerid_contact@odata.bind"] = "/contacts(" + interaction.ServiceRequest.ContactId + ")";
                }
                if (!string.IsNullOrEmpty(interaction.ServiceRequest.Title))
                {
                    sr.Add("title", interaction.ServiceRequest.Title);
                }
                if (!string.IsNullOrEmpty(interaction.ServiceRequest.Description))
                {
                    sr.Add("description", interaction.ServiceRequest.Description);
                }
                if (!string.IsNullOrEmpty(interaction.householdId))
                {
                    sr.Add("hackney_household_incidentid@odata.bind", "/hackney_households(" + interaction.householdId + ")");
                }
                sr.Add("housing_requestcallback", interaction.ServiceRequest.RequestCallback);

                try
                {
                
                    var token = _crmAccessToken.getCRM365AccessToken().Result;
                    _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;
                    _client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                    _client.DefaultRequestHeaders.Add("Prefer",
                        "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
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
                        interaction.ServiceRequest.Id = incidentid;
                        if (!string.IsNullOrWhiteSpace(interaction.ServiceRequest.Description))
                        {
                            var annotationResult = await CreateAnnotation(interaction, _client);
                        }

                    }
                    if (!createResponseIncident.IsSuccessStatusCode)
                    {
                        throw new ServiceRequestException();
                    }

                }
                catch (Exception e)
                {
                    throw e;
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
                if (Utils.NullToString(interaction.subject) != "")
                {
                    tmiJObject["hackney_subjectId@odata.bind"] = "/subjects(" + interaction.subject + ")";
                }
                if (Utils.NullToString(interaction.contactId) != "")
                {
                    tmiJObject["hackney_contactId@odata.bind"] = "/contacts(" + interaction.contactId + ")";
                }
                if (Utils.NullToString(interaction.estateOfficerId) != "")
                {
                    tmiJObject["hackney_estateofficer_createdbyid@odata.bind"] = " /hackney_estateofficers(" + interaction.estateOfficerId + ")";

                }

                if (!string.IsNullOrEmpty(interaction.officerPatchId))
                {
                    tmiJObject["hackney_estateofficerpatchid@odata.bind"] = " /hackney_estateofficerpatchs(" + interaction.officerPatchId + ")";
                }
                if (!string.IsNullOrEmpty(interaction.managerId))
                {
                    tmiJObject["hackney_managerpropertypatchid@odata.bind"] = " /hackney_estatemanagerareas(" + interaction.managerId + ")";
                }
                if (!string.IsNullOrEmpty(interaction.areaName))
                {
                    tmiJObject["hackney_areaname"] = interaction.areaName;
                }
                if (!string.IsNullOrEmpty(interaction.source))
                {
                    //First Dropdown Source
                    tmiJObject.Add("hackney_handleby", interaction.source);
                }
                if (!string.IsNullOrEmpty(interaction.natureofEnquiry))
                {
                    //Second Dropdown
                    tmiJObject.Add("hackney_natureofenquiry", interaction.natureofEnquiry);
                }
                if (interaction.reasonForStartingProcess!=null)
                {
                    //Reason for starting process (if interaction is a process)
                    tmiJObject.Add("hackney_reasonforstartingprocess", interaction.reasonForStartingProcess);
                }
                if (!string.IsNullOrEmpty(interaction.enquirySubject))
                {
                    // Third Dropdown
                    tmiJObject.Add("hackney_enquirysubject", interaction.enquirySubject);
                }
                // Parent Interaction (This is self referencing with TM Interactions
                if (!string.IsNullOrEmpty(interaction.parentInteractionId))
                {
                    tmiJObject.Add("hackney_parent_interactionid@odata.bind", " /hackney_tenancymanagementinteractionses(" + interaction.parentInteractionId + ")");
                }
                // Process Type :- 0 Interaction , 1 TM Process , 2 Post Visit Action
                tmiJObject.Add("hackney_processtype", !string.IsNullOrEmpty(interaction.processType) ? interaction.processType : "0");

                if (interaction.processType != "0")
                {
                    tmiJObject.Add("hackney_process_stage",0);
                }
                // householdId This is require for TM process or TM post Visit Action
                if (!string.IsNullOrEmpty(interaction.householdId))
                {
                    tmiJObject.Add("hackney_household_Interactionid@odata.bind", " /hackney_households(" + interaction.householdId + ")");
                }
                try
                {
                    _logger.LogInformation($"Create Tenancy Management Interaction");
                    var token = _crmAccessToken.getCRM365AccessToken().Result;
                    _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;
                    var incidentquery = HousingAPIQueryBuilder.PostTenancyManagementInteractionQuery();
                    var createResponseInteraction = await _ManageATenancyAPI.postHousingAPI(_client, incidentquery, tmiJObject);

                    if (createResponseInteraction != null)
                    {
                        if (!createResponseInteraction.IsSuccessStatusCode)
                        {
                            _logger.LogError($" Tenancy Management Interaction could not be created  {interaction} ");
                            throw new TenancyInteractionServiceException();
                        }
                        var interactionUri = createResponseInteraction.Headers.GetValues("OData-EntityId")
                            .FirstOrDefault();
                        Guid tmInteractionId = Guid.Empty;
                        if (interactionUri != null)
                        {
                            tmInteractionId = Guid.Parse(interactionUri.Split('(', ')')[1]);
                        }

                        var TenancyManagement = new TenancyManagement
                        {
                            interactionId = tmInteractionId.ToString(),
                            contactId = interaction.contactId,
                            enquirySubject = interaction.enquirySubject,
                            estateOfficerId = interaction.estateOfficerId,
                            subject = interaction.subject,
                            adviceGiven = interaction.adviceGiven,
                            estateOffice = interaction.estateOffice,
                            source = interaction.source,
                            natureofEnquiry = interaction.natureofEnquiry,
                            officerPatchId = interaction.officerPatchId,
                            areaName = interaction.areaName,
                            managerId = interaction.managerId,
                            ServiceRequest = new CRMServiceRequest
                            {
                                TicketNumber = ticketnumber,
                                Id = incidentid,
                                Subject = interaction.ServiceRequest.Subject,
                                ContactId = interaction.ServiceRequest.ContactId,
                                Title = interaction.ServiceRequest.Title,
                                Description = interaction.ServiceRequest.Description
                            },
                            parentInteractionId = interaction.parentInteractionId,
                            processType = interaction.processType,
                            householdId = interaction.householdId
                        };

                        return TenancyManagement;
                    }
                    else
                    {
                        _logger.LogError($" Tenancy Management Interaction could not be created  {interaction} ");
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
                _logger.LogError($" Service Request could not be created  {interaction} ");
                throw new MissingTenancyInteractionRequestException();
            }
        }


        public async Task<CRMServiceRequest> CreateCrmServiceRequest(CRMServiceRequest serviceRequest)
        {
            _logger.LogInformation($"Create Service Request");
            var sr = new JObject();

            if (!string.IsNullOrEmpty(serviceRequest.Subject))
            {
                sr["subjectid@odata.bind"] = "/subjects(" + serviceRequest.Subject + ")";
            }
            if (!string.IsNullOrEmpty(serviceRequest.ContactId))
            {
                sr["customerid_contact@odata.bind"] = "/contacts(" + serviceRequest.ContactId + ")";
            }
            if (!string.IsNullOrEmpty(serviceRequest.Title))
            {
                sr.Add("title", serviceRequest.Title);
            }
            if (!string.IsNullOrEmpty(serviceRequest.Description))
            {
                sr.Add("description", serviceRequest.Description);
            }

            sr.Add("housing_requestcallback", serviceRequest.RequestCallback);

            try
            {
                var token = _crmAccessToken.getCRM365AccessToken().Result;
                _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
                var incidentquery = HousingAPIQueryBuilder.PostIncidentQuery();
                var createResponseIncident = await _ManageATenancyAPI.SendAsJsonAsync(_client, HttpMethod.Post, incidentquery, sr);
                var incidentid = string.Empty;
                var ticketnumber = string.Empty;

                if (createResponseIncident != null)
                {
                    if (createResponseIncident.StatusCode == HttpStatusCode.Created)
                    {
                        JObject createdServiceRequest = JsonConvert.DeserializeObject<JObject>(
                            await createResponseIncident.Content.ReadAsStringAsync());
                        incidentid = createdServiceRequest["incidentid"].ToString();
                        ticketnumber = createdServiceRequest["ticketnumber"].ToString();
                    }
                    if (!createResponseIncident.IsSuccessStatusCode)
                    {
                        throw new ServiceRequestException();
                    }

                    var servicerequest = new CRMServiceRequest
                    {
                        TicketNumber = ticketnumber,
                        Id = incidentid,
                        Subject = serviceRequest.Subject,
                        ContactId = serviceRequest.ContactId,
                        Title = serviceRequest.Title,
                        Description = serviceRequest.Description,
                        RequestCallback = serviceRequest.RequestCallback
                    };

                    return servicerequest;
                }
                else
                {
                    _logger.LogError($" Service Request could not be created  {serviceRequest} ");
                    throw new MissingServiceRequestException();
                }

            }
            catch (Exception e)
            {
                //log error response
                throw e;
            }

        }

        public async Task<object> UpdateTenancyManagementInteraction(TenancyManagement interaction)
        {
            try
            {
                HttpResponseMessage updateResponse = new HttpResponseMessage();
                object annotationResult = null;
                _logger.LogInformation($"Update Tenancy Management Interaction");
                _logger.LogInformation($"Update description which is the notes that is sent from the UI ,Name of the Asset officer who has updated and the current time stamp");
                var token = _crmAccessToken.getCRM365AccessToken().Result;
                string descriptionText = interaction.adviceGiven + "  at " + DateTime.Now.ToString() + " by  " + interaction.estateOfficerName;
                string interactionQuery = HousingAPIQueryBuilder.updateInteractionQuery(interaction.interactionId);
                string incidentUpdateQuery = HousingAPIQueryBuilder.updateIncidentQuery(interaction.ServiceRequest.Id);
                _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                if (interaction.ServiceRequest != null && !string.IsNullOrWhiteSpace(interaction.ServiceRequest.Description))
                {
                    annotationResult = await CreateAnnotation(interaction, _client);
                }
                //status=0 is sent in case we need to close the call status=1 if we need to update the call
                if (interaction.status == 0)
                {

                    JObject tenancyInteraction = new JObject();
                    _logger.LogError($"StateCodeInactive {Constants.StateCodeInactive}");
                    _logger.LogError($"StatusCodeInActive {Constants.StatusCodeInActive}");

                    tenancyInteraction.Add("statuscode", Constants.StatusCodeInActive);
                    tenancyInteraction.Add("statecode", Constants.StateCodeInactive);
                    if (interaction.processType != "0")
                    {
                        tenancyInteraction.Add("hackney_process_stage", interaction.processStage);
                    }

                    tenancyInteraction["hackney_estateofficer_updatedbyid@odata.bind"] = "/hackney_estateofficers(" + interaction.estateOfficerId + ")";
                    tenancyInteraction.Add("modifiedon", DateTime.Now);

                    bool returnResponse = await _ManageATenancyAPI.UpdateObject(_client, interactionQuery, tenancyInteraction);
                    //prepare object to update the incident before closing it
                    JObject incidentObject = new JObject();
                    incidentObject.Add("housing_requestcallback", false);
                    _logger.LogInformation($"Update the callback property of incident entity to false before closing incident");
                    bool updateIncident =
                        await _ManageATenancyAPI.UpdateObject(_client, incidentUpdateQuery, incidentObject);

                    if (returnResponse == false || updateIncident == false)
                    {
                        _logger.LogInformation($"One or more API calls failed while closing incident.");
                        throw new TenancyServiceException();

                    }

                    //closing incident
                    var response = CloseIncident(interaction, _client);
                }
                else
                {
                    JObject tenancyInteraction = new JObject();

                    tenancyInteraction["hackney_estateofficer_updatedbyid@odata.bind"] = "/hackney_estateofficers(" + interaction.estateOfficerId + ")";
                    if(interaction.processType != "0")
                    {
                        tenancyInteraction["hackney_process_stage"] = interaction.processStage;
                    }
                    tenancyInteraction.Add("modifiedon", DateTime.Now);

                    bool returnResponse = await _ManageATenancyAPI.UpdateObject(_client, interactionQuery, tenancyInteraction);
                    //prepare object to update incident callback property
                    JObject incidentObject = new JObject();
                    incidentObject.Add("housing_requestcallback", interaction.ServiceRequest.RequestCallback);
                    _logger.LogInformation($"Update the callback property of incident entity");
                    bool updateIncident =
                        await _ManageATenancyAPI.UpdateObject(_client, incidentUpdateQuery, incidentObject);

                    if (returnResponse == false || updateIncident == false)
                    {
                        _logger.LogInformation($"One or more API calls failed while updating incident.");
                        throw new TenancyServiceException();
                    }
                }
                return new
                {
                    result = buildResponse(annotationResult?.ToString(), interaction)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update Tenancy Management Interaction Error " + ex.Message);
                throw ex;

            }
        }

        public async Task<object> GetTenancyManagementInteraction(string contactId, string personType)
        {
            HttpResponseMessage result = null;
            try
            {

                HttpResponseMessage updateResponse = new HttpResponseMessage();
                _logger.LogInformation($"Update Tenancy Management Interaction");
                _logger.LogInformation($"Update description which is the notes that is sent from the UI ,Name of the Asset officer who has updated and the current time stamp");
                var token = _crmAccessToken.getCRM365AccessToken().Result;
                _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");

                var query = HousingAPIQueryBuilder.getTenancyInteractionDeatils(contactId, personType);

                result = _ManageATenancyAPI.getHousingAPIResponse(_client, query, contactId).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new TenancyServiceException();
                    }

                    var accountRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (accountRetrieveResponse?["value"] != null && accountRetrieveResponse?["value"].Count() > 0)
                    {

                        List<JToken> accountResponse = accountRetrieveResponse["value"].ToList();

                        return new
                        {
                            results = prepareTenancyResultObject(accountResponse)
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
                    _logger.LogError($" Tenancy Incident Details Missing for contact {contactId} of type {personType} ");
                    throw new NullResponseException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get Tenancy Management Interaction Error " + ex.Message);
                throw ex;

            }
        }

        public async Task<object> GetAreaTrayInteractions(string areaId)
        {
            HttpResponseMessage result = null;
            try
            {

                HttpResponseMessage updateResponse = new HttpResponseMessage();
                _logger.LogInformation($"Get Tenancy Management Group Tray Interaction");
                var token = _crmAccessToken.getCRM365AccessToken().Result;
                _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");

                var query = HousingAPIQueryBuilder.getAreaTrayInteractions(areaId);

                result = _ManageATenancyAPI.getHousingAPIResponse(_client, query, areaId).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new TenancyServiceException();
                    }

                    var areaTrayRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (areaTrayRetrieveResponse?["value"] != null && areaTrayRetrieveResponse["value"].Count() > 0)
                    {
                        var areaTrayResponse = areaTrayRetrieveResponse["value"].ToList();

                        return new
                        {
                            results = prepareTenancyResultObject(areaTrayResponse)
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
                    _logger.LogError($" Tenancy Management Group Tray Interaction for missing for areaId {areaId} ");
                    throw new NullResponseException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get Tenancy Management Group Tray Interaction Error " + ex.Message);
                throw ex;

            }
        }

        public async Task<object> TransferCallToAreaAndPatch(TenancyManagement interaction)
        {
            HttpResponseMessage result = null;
            object annotationResult = null;
            var tmiJObject = new JObject();
            try
            {
                HttpResponseMessage updateResponse = new HttpResponseMessage();
                _logger.LogInformation($"Transfer Call To Area And Patch Entry");

                var token = _crmAccessToken.getCRM365AccessToken().Result;
                _client = _hackneyAccountApiBuilder.CreateRequest(token).Result;

                _logger.LogInformation($"update Area and patch for all interactions");
                string interactionQuery = HousingAPIQueryBuilder.updateInteractionQuery(interaction.interactionId);
                //In case manager transfers call to patch we need officerPatchId
                //manager transfers a call from patch to onself--we need estatemanagerareas(managerId)
                //Patch POsts call to manager then managerId for manager Areas -estatemanagerareas(managerId)
                //---------------------------------------------manager------------Patch-----------Area
                //--Call Created/Transferred by/to Manger------Yes------------------Yes-----------Yes
                //---Call created/Transferred by/to  Patch------No-------------------Yes-----------Yes
                if (Utils.NullToString(interaction.estateOfficerId) != "")
                {
                    tmiJObject["hackney_estateofficer_updatedbyid@odata.bind"] = " /hackney_estateofficers(" + interaction.estateOfficerId + ")";
                }
                //manager wants to transfer interaction to a patch. Interaction was currently assigned to manager so association from manager to interaction needs to be removed
                if (!string.IsNullOrEmpty(interaction.officerPatchId) && interaction.assignedToManager && !interaction.assignedToPatch)
                {
                    tmiJObject["hackney_estateofficerpatchid@odata.bind"] = " /hackney_estateofficerpatchs(" + interaction.officerPatchId + ")";
                    //delete association of interaction with manager id
                    string disassociateManagerFromInteraction =
                        HousingAPIQueryBuilder.deleteAssociationOfManagerWithInteraction(interaction.interactionId,
                            interaction.managerId, _client.BaseAddress.ToString());

                    var response = _ManageATenancyAPI.deleteObjectAPIResponse(_client, disassociateManagerFromInteraction).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new TenancyServiceException();
                    }
                    interaction.assignedToManager = false;
                    interaction.managerId = null;
                }
                //manager wants to transfer an interaction from one patch to another or from area tray to a patch and the interaction was not currently assigned to manager
                if (!string.IsNullOrEmpty(interaction.officerPatchId) && !interaction.assignedToManager && !interaction.assignedToPatch)
                {
                    tmiJObject["hackney_estateofficerpatchid@odata.bind"] = " /hackney_estateofficerpatchs(" + interaction.officerPatchId + ")";
                  
                }
                //interaction is currently not assigned to a patch (it is in the area tray) and manager wants to assign to themselves
                if (!string.IsNullOrEmpty(interaction.managerId) && !interaction.assignedToPatch && !interaction.assignedToManager)
                {
                    tmiJObject["hackney_managerpropertypatchid@odata.bind"] = " /hackney_estatemanagerareas(" + interaction.managerId + ")";
                }
                //patch wants to return interaction to their manager. This should remove association of patch to the interaction so they no longer see it in work tray.
                if (!string.IsNullOrEmpty(interaction.managerId) && interaction.assignedToPatch)
                {
                    tmiJObject["hackney_managerpropertypatchid@odata.bind"] = " /hackney_estatemanagerareas(" + interaction.managerId + ")";
                    string disassociatePatchFromInteraction =
                        HousingAPIQueryBuilder.deleteAssociationOfPatchWithInteraction(interaction.interactionId,
                            interaction.managerId, _client.BaseAddress.ToString());

                    var response = _ManageATenancyAPI.deleteObjectAPIResponse(_client, disassociatePatchFromInteraction).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new TenancyServiceException();
                    }
                    interaction.assignedToPatch = false;
                    interaction.officerPatchId = null;
                }
                if (!string.IsNullOrEmpty(interaction.areaName))
                {
                    tmiJObject["hackney_areaname"] = interaction.areaName;
                }
                //set transferred to true
                tmiJObject["hackney_transferred"] = true;
                if (interaction.processStage != 0)
                {
                    //set process stage > 0 - In-progress, 1 - Awaiting manager review, 2 - Approved, 3 - Declined
                    tmiJObject["hackney_process_stage"] = interaction.processStage;
                }

                _logger.LogInformation($"Calling UPdate Object for Transfer Call To Area and Patch");
                var method = new HttpMethod("PATCH");

                HttpResponseMessage returnResponse = await _ManageATenancyAPI.SendAsJsonAsync(_client, method, interactionQuery, tmiJObject);
                if (returnResponse != null)
                {
                    if (!returnResponse.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Error in  Tranfer of call to a Patch/Manager by patch interaction Id {interaction.interactionId}");
                        throw new TenancyServiceException();
                    }

                    _logger.LogInformation($"--create Annotation--");

                    if (interaction.ServiceRequest != null && !string.IsNullOrWhiteSpace(interaction.ServiceRequest.Description))
                    {
                        _client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                        annotationResult = await CreateAnnotation(interaction, _client);
                    }

                    if (annotationResult != null)
                    {
                        dynamic expandoObj = APIActionHelper.convertToExpando(interaction);
                        APIActionHelper.AddProperty(expandoObj, "annotationid", annotationResult);
                        return expandoObj;
                    }

                    return interaction;
                }
                else
                {
                    _logger.LogError($"Transfer call to Path/Area not updated in CRM interactionId {interaction.interactionId}");
                    throw new NullResponseException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Transfer call to Path/Area threw an exception for interactionId  {interaction.interactionId}");
                throw ex;
            }
        }
        private async Task<object> CreateAnnotation(TenancyManagement interaction, HttpClient client)
        {
            try
            {
                string descriptionText = interaction.ServiceRequest.Description + " logged on  " + DateTime.Now.ToString() + " by  " + interaction.estateOfficerName;
                HttpResponseMessage response;
                string annotationId = string.Empty;
                JObject note = new JObject();
                note["notetext"] = descriptionText;
                note["objectid_incident@odata.bind"] = "/incidents(" + interaction.ServiceRequest.Id + ")";
                string requestUrl = "api/data/v8.2/annotations?$select=annotationid";

                response = await _ManageATenancyAPI.SendAsJsonAsync(client, HttpMethod.Post, requestUrl, note);
                if (response == null)
                {
                    _logger.LogError($" Response is null  {interaction.ServiceRequest.Id}");
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
                    _logger.LogError($" Response is null  {interaction.ServiceRequest.Id}");
                    throw new TenancyServiceException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" Create Annotation Error with service reqest  {interaction.ServiceRequest.Id}");
                throw ex;
            }

        }

        private async Task<object> CloseIncident(TenancyManagement interaction, HttpClient client)
        {
            try
            {
                string requestUrl = "/api/data/v8.2/CloseIncident";
                HttpResponseMessage response;
                JObject srClose = new JObject();
                srClose.Add("subject", interaction.ServiceRequest.Description);
                srClose["incidentid@odata.bind"] = "/incidents(" + interaction.ServiceRequest.Id + ")";
                srClose.Add("description", interaction.ServiceRequest.Description);
                JObject resolution = new JObject();
                resolution["IncidentResolution"] = srClose;
                resolution["Status"] = _configuration.CompletedClosureType;

                response = await _ManageATenancyAPI.SendAsJsonAsync(client, HttpMethod.Post, requestUrl, resolution);

                if (!response.IsSuccessStatusCode)
                {
                    throw new TenancyServiceException();
                }

                return response.StatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($" close incident error with service request  {interaction.ServiceRequest.Id}");
                throw new TenancyServiceException();
            }
        }

        private List<dynamic> prepareTenancyResultObject(List<JToken> responseList)
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
                                     areaManagerName = response["ManagerFirstName"]+" "+ response["ManagerLastName"],
                                     officerPatchId = response["_hackney_estateofficerpatchid_value"],
                                     officerPatchName = response["OfficerFirstName"]+" "+response["OfficerLastName"],
                                     areaName = response["hackney_areaname@OData.Community.Display.V1.FormattedValue"],
                                     handledBy = response["hackney_handleby@OData.Community.Display.V1.FormattedValue"],
                                     requestCallBack = response["incident1_x002e_housing_requestcallback"],
                                     contactId = response["_hackney_contactid_value"],
                                     contactName = response["_hackney_contactid_value@OData.Community.Display.V1.FormattedValue"],
                                     contactPostcode = response["contact3_x002e_address1_postalcode"],
                                     contactAddressLine1 = response["contact3_x002e_address1_line1"],
                                     contactAddressLine2 = response["contact3_x002e_address1_line2"],
                                     contactAddressLine3 = response["contact3_x002e_address1_line3"],
                                     contactAddressCity = response["contact3_x002e_address1_city"],
                                     contactBirthDate = response["contact3_x002e_birthdate"],
                                     contactTelephone = response["contact3_x002e_telephone1"],
                                     contactEmailAddress = response["contact3_x002e_emailaddress1"],
                                     contactLarn = response["contact3_x002e_hackney_larn"],
                                     contactUPRN = response["contact3_x002e_hackney_uprn"],
                                     householdID = response["_hackney_household_interactionid_value"],
                                     accountCreatedOn=response["accountCreatedOn"]

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
                                     grp.Key.handledBy,
                                     grp.Key.requestCallBack,
                                     grp.Key.contactId,
                                     grp.Key.contactName,
                                     grp.Key.contactPostcode,
                                     grp.Key.contactAddressLine1,
                                     grp.Key.contactAddressLine2,
                                     grp.Key.contactAddressLine3,
                                     grp.Key.contactAddressCity,
                                     grp.Key.contactBirthDate,
                                     grp.Key.contactTelephone,
                                     grp.Key.contactEmailAddress,
                                     grp.Key.contactLarn,
                                     grp.Key.contactUPRN,
                                     grp.Key.householdID,
                                     grp.Key.accountCreatedOn,
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
                tenancyObj.handledBy = response.handledBy;
                tenancyObj.requestCallBack = response.requestCallBack;
                tenancyObj.contactId = response.contactId;
                tenancyObj.contactName = response.contactName;
                tenancyObj.contactPostcode = response.contactPostcode;
                tenancyObj.contactAddressLine1 = response.contactAddressLine1;
                tenancyObj.contactAddressLine2 = response.contactAddressLine2;
                tenancyObj.contactAddressLine3 = response.contactAddressLine3;
                tenancyObj.contactAddressCity = response.contactAddressCity;
                tenancyObj.contactBirthDate = response.contactBirthDate;
                tenancyObj.contactTelephone = response.contactTelephone;
                tenancyObj.contactEmailAddress = response.contactEmailAddress;
                tenancyObj.contactLarn = response.contactLarn;
                tenancyObj.contactUPRN = response.contactUPRN;
                tenancyObj.householdID = response.householdID;
                tenancyObj.accountCreatedOn = response.accountCreatedOn!=null? response.accountCreatedOn.ToString("yyyy-MM-dd HH:mm:ss") : null;
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



        private object buildResponse(string annotationResult, TenancyManagement interaction)
        {
            return new
            {
                annotationId = annotationResult,
                serviceRequestId = interaction.ServiceRequest.Id,
                interaction.interactionId,
                interaction.ServiceRequest.Description,
                status = interaction.status == 0 ? "Closed" : "InProgress",
                requestCallBack = interaction.ServiceRequest.RequestCallback,
                processStage = interaction.processStage

            };
        }

       
    }

    public class MissingTenancyInteractionRequestException : Exception
    {
    }

    public class TenancyInteractionServiceException : Exception
    {
    }

    public class MissingServiceRequestException : Exception
    {
    }

    public class ServiceRequestException : Exception
    {
    }

    public class TenancyServiceException : System.Exception
    {
    }
    public class NullResponseException : System.Exception
    {
    }

}
