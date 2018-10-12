using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Factories;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Services;
using System.Configuration;
using ManageATenancyAPI.Models;
using Hackney.InterfaceStubs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using MyPropertyAccountAPI.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class LoginController : Controller
    {
        private readonly ILoggerAdapter<TransactionsController> _logger;
        private readonly ILoggerAdapter<LoginActions> _loggerActionAdapter;
        private readonly IHackneyHousingAPICall _hackneyHousingAPICall;
        private readonly IHackneyGetCRM365Token _getCRM365AccessToken;
        private readonly IHackneyHousingAPICallBuilder _hackneyHousingAPICallBuilder;
        private readonly ICryptoMethods _hackneyContractSecurity;

        public LoginController(ILoggerAdapter<TransactionsController> loggerAdapter, ILoggerAdapter<LoginActions> loggerActionAdapter, IHackneyHousingAPICall hackneyHousingAPICall, IHackneyHousingAPICallBuilder hackneyHousingAPICallBuilder, ICryptoMethods cryptoMethods, IOptions<URLConfiguration> config)
        {
            _logger = loggerAdapter;
            _loggerActionAdapter = loggerActionAdapter;
            _hackneyHousingAPICall = hackneyHousingAPICall;
            _getCRM365AccessToken = new HackneyGetCRM365Token(config);
            _hackneyHousingAPICallBuilder = hackneyHousingAPICallBuilder;
            _hackneyContractSecurity = cryptoMethods;          
        }

        [Route("AuthenticateNHOfficers")]
        [HttpGet]
        public async Task<JsonResult> GetAuthenticatedUser(string username, string password)
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                {                  
                       var accountActions = new LoginActions(_loggerActionAdapter, _hackneyHousingAPICall, _hackneyHousingAPICallBuilder, _hackneyContractSecurity,_getCRM365AccessToken);

                    var loginResult = await accountActions.GetAuthenticatedUser(username, password);

                    var json = Json(loginResult);
                    json.StatusCode = Json(loginResult).StatusCode;
                    json.ContentType = "application/json";
                    return json;
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                   {
                    new ApiErrorMessage
                      {
                        developerMessage = "Please provide Username and Password",
                        userMessage = "Please provide Username and Password"
                      }
                  };

                    var jsonResponse = Json(errors);
                    jsonResponse.StatusCode = 400;
                    jsonResponse.ContentType = "application/json";
                    return jsonResponse;
                }
            }
            catch (Exception ex)
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
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }

        }
    }
}
