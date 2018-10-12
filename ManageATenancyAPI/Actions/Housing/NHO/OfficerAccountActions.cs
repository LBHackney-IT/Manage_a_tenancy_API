using Hackney.InterfaceStubs;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models.Housing.NHO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class OfficerAccountActions
    {
        private HttpClient _client;
        private readonly ILoggerAdapter<OfficerAccountActions> _logger;
        private readonly IHackneyHousingAPICall _hackneyHousingAPICall;
        private readonly IHackneyHousingAPICallBuilder _hackneyHousingAPICallBuilder;
        private IHackneyGetCRM365Token _accessToken;
        private readonly ICryptoMethods _hackneyContractSecurity;


        public OfficerAccountActions(ILoggerAdapter<OfficerAccountActions> logger, IHackneyHousingAPICall hackneyHousingAPICall, IHackneyHousingAPICallBuilder hackneyHousingAPICallBuilder, ICryptoMethods cryptoMethods, IHackneyGetCRM365Token accessToken)
        {
            _logger = logger;
            _hackneyHousingAPICall = hackneyHousingAPICall;
            _accessToken = accessToken;
            _hackneyHousingAPICallBuilder = hackneyHousingAPICallBuilder;
            _hackneyContractSecurity = cryptoMethods;
        }

        public async Task<object> CreateOfficerAccount(EstateOfficerAccount estateOfficerAccount)
        {
            
            try
            {
                _logger.LogInformation($"logging estate officer account setup process starts ");

                _logger.LogInformation($"logging the access token request start");



                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _logger.LogInformation($"logging the access token request finished");

                _client = _hackneyHousingAPICallBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                if (await IsUserNameExist(estateOfficerAccount) == true)
                    return  new HttpResponseMessage(HttpStatusCode.Conflict) ;

                _logger.LogInformation($"logging estate officer account setup process stopped due to user already existy ");
                
                var createOfficerAccount = await PersistofficerAccount(estateOfficerAccount);

                return new
                {
                    result = createOfficerAccount
                };

                _logger.LogInformation($"logging estate officer account setup process completed successfully ");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual async Task<JObject> PersistofficerAccount(EstateOfficerAccount estateOfficerAccount)
        {
            try
            {
                _logger.LogInformation($"logging entity creation for estate officer account starts ");

                JObject officerAccount = null;
                JObject officerLoginAccount = null;

                estateOfficerAccount.OfficerLoginAccount.HackneyPassword = _hackneyContractSecurity.Encrypt(estateOfficerAccount.OfficerLoginAccount.HackneyPassword);

                var accountObj = JObject.FromObject(estateOfficerAccount.OfficerAccount);

                var officerAccountCreateResponse =  _hackneyHousingAPICall.postHousingAPI(_client, HousingAPIQueryBuilder.PostEstateOfficerSelectQuery(), accountObj).Result;

                if (officerAccountCreateResponse.StatusCode == HttpStatusCode.Created && officerAccountCreateResponse != null)
                {
                    _logger.LogInformation($"logging entity creation for estate officer account completed successfully");

                    officerAccount = JsonConvert.DeserializeObject<JObject>(await officerAccountCreateResponse.Content.ReadAsStringAsync());

                    officerLoginAccount = await PersistofficerLoginAccount(estateOfficerAccount, officerAccount["hackney_estateofficerid"].ToString());
                }

                var outputresult = await EstateOfficerLoginAccountDetails(officerAccount, officerLoginAccount);

                return (JObject)outputresult;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected virtual async Task<JObject> PersistofficerLoginAccount(EstateOfficerAccount estateOfficerAccount, string estateOfficerId)
        {
            try
            {
                _logger.LogInformation($"logging entity creation for estater office login ccount starts ");

                JObject officerLoginAccount = null;

                var loginAccountObj = JObject.FromObject(estateOfficerAccount.OfficerLoginAccount);

                loginAccountObj.Add("hackney_officerloginId@odata.bind", "/hackney_estateofficers(" + estateOfficerId + ")");

                var officerLoginAccountCreateResponse = _hackneyHousingAPICall.postHousingAPI(_client, HousingAPIQueryBuilder.PostEstateOfficerLoginSelectQuery(), loginAccountObj).Result;

                if (officerLoginAccountCreateResponse.StatusCode == HttpStatusCode.Created && officerLoginAccountCreateResponse != null)
                {
                    _logger.LogInformation($"logging entity creation for estate officer login account completed successfully");

                    officerLoginAccount = JsonConvert.DeserializeObject<JObject>(await officerLoginAccountCreateResponse.Content.ReadAsStringAsync());
                }

                return officerLoginAccount;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<object> DisableAccount(string officerId, string officerLoginId)
        {
            HttpResponseMessage result = null;
            try
            {
                _logger.LogInformation($"logging estate officer account disable steps starts ");

                _logger.LogInformation($"logging the access token request start");



                var accessToken = _accessToken.getCRM365AccessToken().Result;

                _logger.LogInformation($"logging the access token request finished");

                _client = _hackneyHousingAPICallBuilder.CreateRequest(accessToken).Result;
                _client.DefaultRequestHeaders.Add("Prefer", "return=representation");

                var disableOfficerAccount = await DisableofficerAccount(officerId, officerLoginId);

                return new
                {
                    result = disableOfficerAccount
                };

                _logger.LogInformation($"logging estate officer account disable steps completed successfully ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual async Task<JObject> DisableofficerAccount(string officerId, string officerLoginId)
        {
            try
            {
                _logger.LogInformation($"logging estater officer account to inactive steps starts ");

                JObject officerAccount = null;
                JObject officerLoginAccount = null;
                

                var accountObj = JObject.FromObject(new DisableAccount() { statecode = Constants.StateCodeInactive, statuscode = Constants.StatusCodeInActive });

                var officerAccountCreateResponse = _hackneyHousingAPICall.SendAsJsonAsync(_client, new HttpMethod("PATCH"), HousingAPIQueryBuilder.UpdateEstateOfficerSelect(officerId), accountObj).Result;

                if (officerAccountCreateResponse.StatusCode == HttpStatusCode.OK && officerAccountCreateResponse != null)
                {
                    _logger.LogInformation($"logging entity update for estater office account completed successfully");

                    officerAccount = JsonConvert.DeserializeObject<JObject>(await officerAccountCreateResponse.Content.ReadAsStringAsync());

                    officerLoginAccount = await DisableofficerLoginAccount(officerLoginId);
                }

                var outputresult = await EstateOfficerLoginAccountDetails(officerAccount, officerLoginAccount);

                return (JObject)outputresult;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected virtual async Task<JObject> DisableofficerLoginAccount(string estateOfficerLoginId)
        {            
            try
            {
                _logger.LogInformation($"logging entity creation for estater office login ccount details starts ");

                JObject officerLoginAccount = null;

                var loginAccountObj = JObject.FromObject(new DisableAccount() { statecode = Constants.StateCodeInactive, statuscode = Constants.StatusCodeInActive });                             

                var officerLoginAccountCreateResponse = _hackneyHousingAPICall.SendAsJsonAsync(_client, new HttpMethod("PATCH"), HousingAPIQueryBuilder.UpdateEstateOfficerLoginQuery(estateOfficerLoginId), loginAccountObj).Result;

                if (officerLoginAccountCreateResponse.StatusCode == HttpStatusCode.OK && officerLoginAccountCreateResponse != null)
                {
                    _logger.LogInformation($"logging entity creation for estate officer login account details completed successfully");

                    officerLoginAccount = JsonConvert.DeserializeObject<JObject>(await officerLoginAccountCreateResponse.Content.ReadAsStringAsync());
                }

                return officerLoginAccount;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected virtual async Task<Boolean> IsUserNameExist(EstateOfficerAccount estateOfficerAccount)
        {
            var result = _hackneyHousingAPICall.getHousingAPIResponse(_client, HousingAPIQueryBuilder.GetOfficerLoginUserNameQuery(estateOfficerAccount.OfficerLoginAccount.HackneyUsername), estateOfficerAccount.OfficerLoginAccount.HackneyUsername).Result;

            var response = JsonConvert.DeserializeObject<JObject>(result.Content.ReadAsStringAsync().Result);

            var output = response["value"].Count() > 0 ? true : false;

            return await Task.FromResult(output);

        }

        protected virtual async Task<JObject> EstateOfficerLoginAccountDetails(JObject officerAccount, JObject officerLoginAccount)
        {
            var result = new
            {
                EstateOfficerid = officerAccount["hackney_estateofficerid"],
                Name = officerAccount["hackney_name"],
                FirstName = officerAccount["hackney_firstname"],
                LastName = officerAccount["hackney_lastname"],
                EmailAddress = officerAccount["hackney_emailaddress"],
                OfficerAccountStatus = AccountStatus(officerAccount),
                EstateOfficerLoginId = officerLoginAccount["hackney_estateofficerloginid"],
                UserName = officerLoginAccount["hackney_username"],
                LoginAccountStatus = AccountStatus(officerLoginAccount)
            };

            return await Task.FromResult(JObject.FromObject(result));

        }

        private string AccountStatus(JObject officerLoginAccount)
        {
            return officerLoginAccount["statecode"].ToString() == "1" && officerLoginAccount["statuscode"].ToString() == "2" ? "InActive" : "Active";
        }
    }
   

}



public class AccountServiceException : System.Exception { }



