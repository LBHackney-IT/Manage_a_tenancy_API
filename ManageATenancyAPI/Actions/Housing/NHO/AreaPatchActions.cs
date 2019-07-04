using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class AreaPatchActions 
    {
        private HttpClient _client;
        private IHackneyGetCRM365Token _accessToken;
        private IHackneyHousingAPICallBuilder _housingApiBuilder;
        private IHackneyHousingAPICall _housingApi;
        private ILoggerAdapter<AreaPatchActions> _logger;
        
        public AreaPatchActions(ILoggerAdapter<AreaPatchActions> logger,
                                IHackneyHousingAPICallBuilder apiCallBuilder,
                                IHackneyHousingAPICall apiCall, IHackneyGetCRM365Token accessToken)
        {
            _client = new HttpClient();
            _accessToken = accessToken;
            _housingApiBuilder = apiCallBuilder;
            _housingApi = apiCall;
            _logger = logger;
        }


        public async Task<object> GetAreaPatch(string postCode, string UPRN)
        {
            HttpResponseMessage result = null;

            try
            {   
                _logger.LogInformation($"GetAreaPatch Action requested Started");


                var accessToken = _accessToken.getCRM365AccessToken().Result;
                                
                _client = _housingApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");


                var query = HousingAPIQueryBuilder.GetAreaPatch(postCode, UPRN);

                result = _housingApi.getHousingAPIResponse(_client, query, postCode).Result;

                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new AreaPatchServiceException();
                    }

                    var areaPatchRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (areaPatchRetrieveResponse?["value"] != null && areaPatchRetrieveResponse["value"].Count() > 0)
                    {
                        JToken areaPatchResponse = areaPatchRetrieveResponse["value"].FirstOrDefault();
                        return new
                        {
                            result = await AreaPatch(areaPatchResponse)
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
                    _logger.LogError($"AreaPatch was not found for postcode");
                     throw new MissingAreaPatchServiceException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAreaPatch Errored " + ex.Message);
                throw ex;

            }
        }

        public async Task<object> GetAllOfficersPerArea(string areaId)
        {
            HttpResponseMessage result = null;
            try
            {
                HttpResponseMessage updateResponse = new HttpResponseMessage();
                _logger.LogInformation($"GetAsync All Officers Per Area");

                var accessToken = _accessToken.getCRM365AccessToken().Result;
                _client = _housingApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
                var query = HousingAPIQueryBuilder.getAllOfficersPerArea(areaId);

                result = _housingApi.getHousingAPIResponse(_client, query, areaId).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new AreaPatchServiceException();
                    }

                    var officersPerArea = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (officersPerArea?["value"] != null && officersPerArea["value"].Count() > 0)
                    {
                        var officersPerAreaResponse = officersPerArea["value"].ToList();
                        return new
                        {
                            results = prepareAllOfficersPerAreaResponse(officersPerAreaResponse)
                        };
                    }
                    else
                    {
                        return new
                        {
                            results = new object()
                        };
                    }

                }
                else
                {
                    _logger.LogError($" All Officers Per Area  missing for Area {areaId} ");
                    throw new MissingAreaPatchServiceException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"All Officers Per Area Error " + ex.Message);
                throw ex;
            }
        }

        public async Task<object> UpdatePatchOrManager(OfficerAreaPatch officerAreaPatch)
        {
            HttpResponseMessage result = null;
            try
            {
                HttpResponseMessage updateResponse = new HttpResponseMessage();


                var accessToken = _accessToken.getCRM365AccessToken().Result;
                _client = _housingApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");
                JObject updatePatchOrArea = new JObject();

                var query = string.Empty;

                if (officerAreaPatch.isUpdatingPatch)
                {
                    if (officerAreaPatch.deleteExistingRelationship)
                    {
                        _logger.LogInformation(
                            $"Delete association of patch {officerAreaPatch.patchId} with officer {officerAreaPatch.officerId} ");
                        query = HousingAPIQueryBuilder.deleteAssociationOfOfficerWithPatch(officerAreaPatch.patchId,
                            officerAreaPatch.officerId, _client.BaseAddress.ToString());
                        var response = _housingApi.deleteObjectAPIResponse(_client, query).Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new UpdatePatchServiceException();
                        }

                        return new
                        {
                            id = officerAreaPatch.patchId,
                        };
                    }
                    else
                    {
                        _logger.LogInformation(
                            $"Updating patch with id {officerAreaPatch.patchId} with officer with id {officerAreaPatch.officerId} ");
                        query = HousingAPIQueryBuilder.updateOfficerAssociatedWithPatch(officerAreaPatch.patchId);

                        updatePatchOrArea["hackney_patchid@odata.bind"] =
                            "/hackney_estateofficers(" + officerAreaPatch.officerId + ")";
                        updatePatchOrArea["hackney_updatedby@odata.bind"] =
                            "/hackney_estateofficers(" + officerAreaPatch.updatedByOfficer + ")";
                    }
                }
                else
                {
                    if (officerAreaPatch.deleteExistingRelationship)
                    {
                        _logger.LogInformation(
                            $"Delete association of area {officerAreaPatch.areamanagerId} with officer {officerAreaPatch.officerId} ");
                        query = HousingAPIQueryBuilder.deleteAssociationOfOfficerWithArea(officerAreaPatch.areamanagerId,
                            officerAreaPatch.officerId, _client.BaseAddress.ToString());
                        var response = _housingApi.deleteObjectAPIResponse(_client, query).Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new UpdatePatchServiceException();
                        }

                        return new
                        {
                            id = officerAreaPatch.areamanagerId
                            
                        };
                    }
                    _logger.LogInformation(
                        $"Updating area manager with id {officerAreaPatch.areamanagerId} with officer with id {officerAreaPatch.officerId} ");
                     query = HousingAPIQueryBuilder.updateOfficerAssociatedWithAreaAsManager(officerAreaPatch.areamanagerId);
                    
                    updatePatchOrArea["hackney_managerareaid@odata.bind"] =
                        "/hackney_estateofficers(" + officerAreaPatch.officerId + ")";
                    updatePatchOrArea["hackney_updatedby@odata.bind"] =
                        "/hackney_estateofficers(" + officerAreaPatch.updatedByOfficer + ")";
                }

                result = _housingApi.SendAsJsonAsync(_client, new HttpMethod("PATCH"), query, updatePatchOrArea).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new UpdatePatchServiceException();
                    }

                    var updatedPatchOrArea =
                        JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);

                    if (officerAreaPatch.isUpdatingPatch)
                    {
                        return new
                        {
                            id = updatedPatchOrArea["hackney_estateofficerpatchid"],
                            patchName = updatedPatchOrArea["hackney_name"]
                        };

                    }
                    else
                    {
                        return new
                        {
                            id = updatedPatchOrArea["hackney_estatemanagerareaid"],
                            areaName = updatedPatchOrArea["hackney_name"]
                        };
                    }
                }
                else
                {
                    _logger.LogError($" Missing result for updating patch/area");
                    throw new MissingResultUpdatePatchException();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured during execution on method UpdatePatch() - " + ex.Message);
                throw ex;
            }
        }

        public async Task<object> GetAllOfficersThatAreNotAssignedToAPatchOrArea()
        {
            HttpResponseMessage result = null;
            try
            {
                _logger.LogInformation($"GetAsync All Officers Per Area");

                var accessToken = _accessToken.getCRM365AccessToken().Result;
                _client = _housingApiBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
                var query = HousingAPIQueryBuilder.getAllOfficersThatAreNotAssignedToPatchOrArea();

                result = _housingApi.getHousingAPIResponse(_client, query, "notAssignedToPatchOrArea").Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new GetAllUnassignedOfficersServiceException();
                    }

                    var unassignedOfficers = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (unassignedOfficers?["value"] != null && unassignedOfficers["value"].Count() > 0)
                    {
                        var unnasignedOfficersResponse = unassignedOfficers["value"].ToList();
                        return new
                        {
                            results = prepareAllUassignedOfficers(unnasignedOfficersResponse)
                        };
                    }
                    else
                    {
                        return new
                        {
                            results = new object[] { }
                        };
                    }

                }
                else
                {
                    _logger.LogError($" GetAsync all unassigned officers result missing ");
                    throw new MissingResultForGetAllUnassignedOfficersException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured during the execution of method GetAllOfficersThatAreNotAssignedToAPatchOrArea() - " + ex.Message);
                throw ex;
            }
        }


         


        private async Task<AreaPatch> AreaPatch(JToken areaPatchResponse)
        {
            var result = new AreaPatch
            {
                
                HackneyAreaId = !string.IsNullOrWhiteSpace((string)areaPatchResponse["hackney_areaname"])? (string)areaPatchResponse["hackney_areaname"]: null,
                HackneyareaName = !string.IsNullOrWhiteSpace((string)areaPatchResponse["hackney_areaname@OData.Community.Display.V1.FormattedValue"])? (string)areaPatchResponse["hackney_areaname@OData.Community.Display.V1.FormattedValue"] :  null,
                HackneyPropertyReference = !string.IsNullOrWhiteSpace((string)areaPatchResponse["hackney_propertyreference"])? (string)areaPatchResponse["hackney_propertyreference"] : null,
                HackneyPostCode = !string.IsNullOrWhiteSpace((string)areaPatchResponse["hackney_postcode"])? (string)areaPatchResponse["hackney_postcode"]: null,
                HackneyllpgReference = !string.IsNullOrWhiteSpace((string)areaPatchResponse["hackney_llpgref"])? (string)areaPatchResponse["hackney_llpgref"]: null,
                HackneyWardId = !string.IsNullOrWhiteSpace((string)areaPatchResponse["hackney_ward"])? (string)areaPatchResponse["hackney_ward"]: null,
                HackneyWardName = !string.IsNullOrWhiteSpace((string)areaPatchResponse["hackney_ward@OData.Community.Display.V1.FormattedValue"])? (string)areaPatchResponse["hackney_ward@OData.Community.Display.V1.FormattedValue"] : null,
                HackneyEstateofficerPropertyPatchId = !string.IsNullOrWhiteSpace((string)areaPatchResponse["_hackney_estateofficerpropertypatchid_value"])? (string)areaPatchResponse["_hackney_estateofficerpropertypatchid_value"]:null,
                HackneyEstateofficerPropertyPatchName = areaPatchResponse["OfficerFirstName"] + " " + areaPatchResponse["OfficerLastName"],
                HackneyManagerPropertyPatchId = !string.IsNullOrWhiteSpace((string)areaPatchResponse["_hackney_managerpropertypatchid_value"])? (string)areaPatchResponse["_hackney_managerpropertypatchid_value"] : null,
                HackneyManagerPropertyPatchName = areaPatchResponse["ManagerFirstName"] + " " + areaPatchResponse["ManagerLastName"],
                HackneyEstateOfficerId = !string.IsNullOrWhiteSpace((string)areaPatchResponse["estateOfficerId"]) ? (string)areaPatchResponse["estateOfficerId"] : null
                
            };

            return await Task.FromResult(result);
        }


        private List<dynamic> prepareAllOfficersPerAreaResponse(List<JToken> officers)
        {
           
            var areaManager = (from response in officers
                               group response by new
                               {
                                   areaName = response["hackney_name"],
                                   areaId = response["hackney_areaname"],
                                   managerPropertyPatchId = response["_hackney_managerpropertypatchid_value"],
                                   managerPropertyPatchName = response["_hackney_managerpropertypatchid_value@OData.Community.Display.V1.FormattedValue"],
                                   areaManagerName = response["hackney_estatemanagerarea2_x002e_hackney_managerareaid@OData.Community.Display.V1.FormattedValue"],
                                   areamanagerId = response["hackney_estatemanagerarea2_x002e_hackney_managerareaid"],
                                   isaManager = true
                               } into grp
                               select new
                               {
                                   grp.Key.managerPropertyPatchId,
                                   grp.Key.managerPropertyPatchName,
                                   grp.Key.areaName,
                                   grp.Key.areaId,
                                   grp.Key.areaManagerName,
                                   grp.Key.areamanagerId,
                                   grp.Key.isaManager,
                                   Patches = grp.ToList()
                               });
            var officersList = new List<dynamic>();
            foreach (dynamic manager in areaManager)
            {
                dynamic OfficersObj = new ExpandoObject();

                OfficersObj.propertyAreaPatchId = "";
                OfficersObj.estateOfficerPropertyPatchId = "";
                OfficersObj.estateOfficerPropertyPatchName = "";
                OfficersObj.llpgReferenece = "";
                OfficersObj.patchId = "";
                OfficersObj.patchName = "";
                OfficersObj.propetyReference = "";
                OfficersObj.wardName = "";
                OfficersObj.wardId = "";
                OfficersObj.areaName = manager.areaName;
                OfficersObj.areaId = manager.areaId;
               
                OfficersObj.managerPropertyPatchId = manager.managerPropertyPatchId;
                OfficersObj.managerPropertyPatchName = manager.managerPropertyPatchName;
                OfficersObj.areaManagerName = manager.areaManagerName;
                OfficersObj.areamanagerId = manager.areamanagerId;
                OfficersObj.isaManager = true;
                OfficersObj.officerId = manager.managerPropertyPatchId;
                OfficersObj.officerName = manager.areaManagerName + " (Area Manager)";
                officersList.Add(OfficersObj);
                foreach (dynamic patch in manager.Patches)
                {
                    dynamic patchObj = new ExpandoObject();
                    patchObj.propertyAreaPatchId = patch["hackney_propertyareapatchid"];
                    patchObj.estateOfficerPropertyPatchId = patch["_hackney_estateofficerpropertypatchid_value"];
                    patchObj.estateOfficerPropertyPatchName = patch["_hackney_estateofficerpropertypatchid_value@OData.Community.Display.V1.FormattedValue"];
                    patchObj.llpgReferenece = patch["hackney_llpgref"];
                    patchObj.patchId = patch["hackney_estateofficerpatch1_x002e_hackney_patchid"];
                    patchObj.patchName = patch["hackney_estateofficerpatch1_x002e_hackney_patchid@OData.Community.Display.V1.FormattedValue"];
                    patchObj.propetyReference = patch["hackney_propertyreference"];
                    patchObj.wardName = patch["hackney_ward@OData.Community.Display.V1.FormattedValue"];
                    patchObj.wardId = patch["hackney_ward"];
                    patchObj.areaName = patch["hackney_name"];
                    patchObj.areaId = patch["hackney_areaname"];
                    patchObj.managerPropertyPatchId = patch["_hackney_managerpropertypatchid_value"];
                    patchObj.managerPropertyPatchName = patch["_hackney_managerpropertypatchid_value@OData.Community.Display.V1.FormattedValue"];
                    patchObj.areaManagerName = patch["hackney_estatemanagerarea2_x002e_hackney_managerareaid@OData.Community.Display.V1.FormattedValue"];
                    patchObj.areamanagerId = patch["hackney_estatemanagerarea2_x002e_hackney_managerareaid"];
                    patchObj.isaManager = false;
                    patchObj.officerId = patch["_hackney_estateofficerpropertypatchid_value"];
                    patchObj.officerName = patch["hackney_estateofficerpatch1_x002e_hackney_patchid@OData.Community.Display.V1.FormattedValue"];
                    officersList.Add(patchObj);
                }
            }
            return officersList;
        }

        private List<dynamic> prepareAllUassignedOfficers(List<JToken> officers)
        {
            var unassignedOfficersList = new List<dynamic>();
            foreach (var officer in officers)
            {
                dynamic officersObj = new ExpandoObject();
                officersObj.firstName = officer["hackney_firstname"];
                officersObj.lastName = officer["hackney_lastname"];
                officersObj.fullName = officer["hackney_name"];
                officersObj.officerId = officer["hackney_estateofficerid"];
                unassignedOfficersList.Add(officersObj);
            }
            return unassignedOfficersList;
        }
    }




    public class AreaPatchServiceException : Exception
    {
    }

    public class UpdatePatchServiceException : Exception{}
    public class MissingResultUpdatePatchException : Exception { }

    public class MissingAreaPatchServiceException : Exception
    {
    }
    public class GetAllUnassignedOfficersServiceException : Exception
    {
    }
    public class MissingResultForGetAllUnassignedOfficersException : Exception
    {
    }
}

