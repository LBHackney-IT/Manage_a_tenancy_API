using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Services.Housing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hackney.InterfaceStubs;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class LoginActions
    {
        private HttpClient _client;
        private readonly ILoggerAdapter<LoginActions> _logger;
        private readonly IHackneyHousingAPICall _hackneyHousingAPICall;
        private IHackneyGetCRM365Token _accessToken;
        private readonly IHackneyHousingAPICallBuilder _hackneyHousingAPICallBuilder;
        private readonly ICryptoMethods _hackneyContractSecurity;
       
        public LoginActions(ILoggerAdapter<LoginActions> logger, IHackneyHousingAPICall hackneyHousingAPICall, IHackneyHousingAPICallBuilder hackneyHousingAPICallBuilder, ICryptoMethods cryptoMethods,IHackneyGetCRM365Token accessToken)
        {
            _logger = logger;
            _hackneyHousingAPICall = hackneyHousingAPICall;
            _accessToken = accessToken;
            _hackneyHousingAPICallBuilder = hackneyHousingAPICallBuilder;
            _hackneyContractSecurity = cryptoMethods;
        }

        public async Task<object> GetAuthenticatedUser(string username, string Password)
        {
            //HttpResponseMessage result = null;
            try
            {
                _logger.LogInformation($"Getting AuthenticateUser for NHO Estate Officer Login {username}");


                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _client = _hackneyHousingAPICallBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");


                
                var query = HousingAPIQueryBuilder.getAuthenticatedUserQuery(username, _hackneyContractSecurity.Encrypt(Password));

                var result = _hackneyHousingAPICall.getHousingAPIResponse(_client, query, username).Result;
                if (result != null)
                {
                    if (!result.IsSuccessStatusCode)
                    {
                        throw new LoginServiceException();
                    }

                    var loginRetrieveResponse = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);
                    if (loginRetrieveResponse?["value"] != null && loginRetrieveResponse["value"].Count() > 0)
                    {

                        dynamic accountResponse = loginRetrieveResponse["value"].FirstOrDefault();

                        return new
                        {
                            result = buildResponse(accountResponse)
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
                    _logger.LogError($" NHO Login Action error method name -GetAuthenticatedUser(): No result is returned from the service");
                    throw new MissingLoginResultException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($" NHO Login Action error method name -GetAuthenticatedUser()" + ex.Message);
                throw;
            }
        }

        private object buildResponse(dynamic loginResponse)
        {
           
            return new
            {
                estateOfficerLoginId = loginResponse["hackney_estateofficerloginid"] != null ? loginResponse["hackney_estateofficerloginid"].ToString().Trim() : null,
                officerId = loginResponse["_hackney_officerloginid_value"] != null ? loginResponse["_hackney_officerloginid_value"].ToString().Trim() : null,
                firstName = loginResponse["hackney_estateofficer1_x002e_hackney_firstname"] != null ? loginResponse["hackney_estateofficer1_x002e_hackney_firstname"] : null,
                surName = loginResponse["hackney_estateofficer1_x002e_hackney_lastname"] != null ? loginResponse["hackney_estateofficer1_x002e_hackney_lastname"] : null,
                username = loginResponse["hackney_username"] != null ? loginResponse["hackney_username"] : null,
                fullName = loginResponse["hackney_estateofficer1_x002e_hackney_name"] != null ? loginResponse["hackney_estateofficer1_x002e_hackney_name"] : null,
                isManager = loginResponse["managerId"] != null ? true : false,
                areamanagerId = GetAreamanagerId(loginResponse),
                officerPatchId = loginResponse["officerPatchId"] != null ? loginResponse["officerPatchId"] : null,
                areaId = GetAreaId(loginResponse),
            };
        }

        private static dynamic GetAreamanagerId(dynamic loginResponse)
        {
            dynamic areamanagerId =null ;
            if (loginResponse["managerId"] != null)
            {
                areamanagerId = loginResponse["managerId"];
            }
            else if (loginResponse["OfficermanagerId"] != null)
            {
                areamanagerId = loginResponse["OfficermanagerId"];
            }
            return areamanagerId;
        }

        private static dynamic GetAreaId(dynamic loginResponse)
        {
            dynamic areaId = null;
            if (loginResponse["OfficerAreaId"] != null)
            {
                areaId = loginResponse["OfficerAreaId"];
            }
            else if (loginResponse["AreaId"] != null)
            {
                areaId = loginResponse["AreaId"] ;
            }
            return areaId;
        }
    }

    public class LoginServiceException : System.Exception
    {
    }
    public class MissingLoginResultException : System.Exception
    {
    }
}
