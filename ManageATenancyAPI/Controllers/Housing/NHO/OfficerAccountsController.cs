using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Validators;
using LBH.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Hackney.InterfaceStubs;
using ManageATenancyAPI.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using MyPropertyAccountAPI.Configuration;


namespace ManageATenancyAPI.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class OfficerAccountsController : Controller
    {

        private readonly ILoggerAdapter<OfficerAccountsController> _logger;
        private readonly ILoggerAdapter<OfficerAccountActions> _loggerActionAdapter;
        private readonly IHackneyHousingAPICall _hackneyHousingAPICall;
        private readonly IHackneyGetCRM365Token _getCRM365AccessToken;
        private readonly IHackneyHousingAPICallBuilder _hackneyHousingAPICallBuilder;
        private readonly ICryptoMethods _hackneyContractSecurity;

        public OfficerAccountsController(ILoggerAdapter<OfficerAccountsController> loggerAdapter, ILoggerAdapter<OfficerAccountActions> loggerActionAdapter, IHackneyHousingAPICall hackneyHousingAPICall, IHackneyHousingAPICallBuilder hackneyHousingAPICallBuilder, ICryptoMethods cryptoMethods, IOptions<URLConfiguration> config)
        {
            _logger = loggerAdapter;
            _loggerActionAdapter = loggerActionAdapter;           
            _hackneyHousingAPICall = hackneyHousingAPICall;
            _getCRM365AccessToken = new HackneyGetCRM365Token(config);
            _hackneyHousingAPICallBuilder = hackneyHousingAPICallBuilder;
            _hackneyContractSecurity = cryptoMethods;
        }


        //POST: api/UserAccounts
        /// <summary>
        /// Creates a new Estate Officer Account
        /// </summary>
        /// <returns>Officers Id, FirstName, LastName, UserName</returns>
        [Route("CreateOfficerAccount")]
        [HttpPost]
        public async Task<IActionResult> CreateOfficerAccount([FromBody] EstateOfficerAccount estateOfficerAccount)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidateEstateOfficerAccount(estateOfficerAccount);
                if (validateResult.Valid)
                {
                    var accountActions = new OfficerAccountActions(_loggerActionAdapter, _hackneyHousingAPICall,  _hackneyHousingAPICallBuilder, _hackneyContractSecurity,_getCRM365AccessToken);

                    var accountCreateresponse = await accountActions.CreateOfficerAccount(estateOfficerAccount);

                    var json = Json(accountCreateresponse);

                    var response = Utils.IsuserNameConflict(json.Value.ToString());

                    json.StatusCode = response == false ? 201 : 409;

                    json.ContentType = "application/json";

                    return json;
                    
                }else{

                    var errors = validateResult.ErrorMessages.Select(error => new ApiErrorMessage
                    {
                        developerMessage = error,
                        userMessage = error
                    }).ToList();
                    var jsonResponse = Json(errors);
                    jsonResponse.StatusCode = 400;
                    jsonResponse.ContentType = "application/json";
                    return jsonResponse;

                }
            }
            catch (AccountServiceException ex) 
            {

                var errors = new List<ApiErrorMessage>
                {
                    new ApiErrorMessage
                    {
                        developerMessage = ex.Message,
                        userMessage = "We had some problems processing your request"
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);                
                json.ContentType = "application/json";
                return json;
            }           
        }


        //POST: api/UserAccounts
        /// <summary>
        /// Disable Estate Officer Account
        /// </summary>
        /// <returns>Officers Id, FirstName, LastName, UserName</returns>
        [Route("DisableOfficerAccount")]
        [HttpPatch]
        public async Task<IActionResult> DisableOfficerAccount(string officerId, string officerLoginId)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidateOfficerAccountIds(officerId, officerLoginId);
                if (validateResult.Valid)
                {
                    var accountActions = new OfficerAccountActions(_loggerActionAdapter, _hackneyHousingAPICall, _hackneyHousingAPICallBuilder, _hackneyContractSecurity,_getCRM365AccessToken);

                    var createContact = await accountActions.DisableAccount(officerId, officerLoginId);

                    var json = Json(createContact);

                    json.StatusCode = 204;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {

                    var errors = validateResult.ErrorMessages.Select(error => new ApiErrorMessage
                    {
                        developerMessage = error,
                        userMessage = error
                    }).ToList();
                    var jsonResponse = Json(errors);
                    jsonResponse.StatusCode = 400;
                    jsonResponse.ContentType = "application/json";
                    return jsonResponse;

                }
            }
            catch (AccountServiceException ex)
            {

                var errors = new List<ApiErrorMessage>
                {
                    new ApiErrorMessage
                    {
                        developerMessage = ex.Message,
                        userMessage = "We had some problems processing your request"
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);                
                json.ContentType = "application/json";
                return json;
            }          
        }

    }
}