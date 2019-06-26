using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Validators;
using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Factories;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using Microsoft.AspNetCore.Mvc;
using ManageATenancyAPI.Actions.Housing;
using ManageATenancyAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Controllers.Housing
{
    [Authorize]
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class TransactionsController : Controller
    {
        private readonly ILoggerAdapter<TransactionsController> _logger;
        private readonly IHackneyHousingAPICall _accountsApiCall;
        private readonly IHackneyGetCRM365Token _accessToken;
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;
        private readonly IHackneyHousingAPICallBuilder _leaseAPICallBuilder;
        private readonly ILoggerAdapter<TransactionsActions> _loggerActionAdapter;
        public TransactionsController(ILoggerAdapter<TransactionsController> loggerAdapter, ILoggerAdapter<TransactionsActions> loggerActionAdapter, IOptions<URLConfiguration> config)
        {
          
         
            var serviceFactory = new HackneyAccountsServiceFactory();
            _accountsApiCall = serviceFactory.build();
            _logger = loggerAdapter;
           _accessToken = new HackneyGetCRM365Token(config);
            _leaseAPICallBuilder = new HackneyHousingAPICallBuilder(config, _apiBuilderLoggerAdapter);
            _loggerActionAdapter = loggerActionAdapter;
        }

        [HttpGet]
        public async Task<JsonResult> GetTransactionsByTagReference(string tagReference)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();

                var validateResult = validator.ValidateTagReference(tagReference);
                if (validateResult.Valid)
                {

                    var transactionsActions = new TransactionsActions(_loggerActionAdapter, _leaseAPICallBuilder, _accountsApiCall,_accessToken);

                    var transactionsData = await transactionsActions.GetTransactionsByTagReference(tagReference);


                    var json = Json(transactionsData);
                    json.StatusCode = Json(transactionsData).StatusCode;
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
            catch (TransactionsServiceException ex)
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