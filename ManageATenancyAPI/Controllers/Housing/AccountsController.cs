using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Factories;
using ManageATenancyAPI.Validators;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Controllers
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class AccountsController : Controller
    {

        private readonly ILoggerAdapter<AccountsController> _logger;
        private readonly IHackneyHousingAPICall _accountsApiCall;
        private readonly IHackneyUHWWarehouseService _accountsrepositoryserviceApiCall;
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;
        private readonly IHackneyHousingAPICallBuilder _leaseAPICallBuilder;
        private readonly IPostcodeValidator _postcodeValidator;
        private readonly IHackneyGetCRM365Token _accessToken;
        private readonly IHousingQueryParameterValidator _propertyReferenceNumber;
        private readonly ILoggerAdapter<AccountActions> _loggerActionAdapter;
        public AccountsController(ILoggerAdapter<AccountsController> loggerAdapter, ILoggerAdapter<AccountActions> loggerActionAdapter, IUHWWarehouseRepository uhwWarehouseRepository, IOptions<URLConfiguration> config)
        {
        
            var serviceFactory = new HackneyAccountsServiceFactory();
            _accountsApiCall = serviceFactory.build();

            var serviceRepositoryFactory = new HackneyUHWWarehouseServiceFactory();
            _accountsrepositoryserviceApiCall = serviceRepositoryFactory.build(uhwWarehouseRepository, loggerActionAdapter);

            _logger = loggerAdapter;
            _accessToken = new HackneyGetCRM365Token(config);
            _leaseAPICallBuilder = new HackneyHousingAPICallBuilder(config, _apiBuilderLoggerAdapter);
            _postcodeValidator = new PostcodeValidator();
            _propertyReferenceNumber = new HousingQueryParameterValidator();
            _loggerActionAdapter = loggerActionAdapter;
        }

        
        /// <summary>
        /// GetTagReferencenumber.
        /// </summary>
        /// <param name="hackneyhomesId">Payment reference number for Account</param>
        /// <returns>Returns tag reference number</returns>
        [Route("GetTagReferencenumber")]
        [HttpGet]
        public async Task<JsonResult> GetTagReferencenumber(string hackneyhomesId)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();

                var validateResult = validator.ValidatehackneyhomesId(hackneyhomesId);
                if (validateResult.Valid)
                {
                    var accountActions = new AccountActions(_loggerActionAdapter, _leaseAPICallBuilder, _accountsApiCall, _accountsrepositoryserviceApiCall,_accessToken);

                    var tagReference = accountActions.GetTagReferencenumber(hackneyhomesId).Result;


                    var json = Json(tagReference);
                    json.StatusCode = Json(tagReference).StatusCode;
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

        /// <summary>
        /// Get account details by tag reference or paris ref
        /// </summary>
        /// <param name="referencenumber">Payment reference or tag reference number for Account</param>
        /// <returns>Returns account details in Json Object</returns>
        [Route("AccountDetailsByPaymentorTagReference")]
        [HttpGet]
        public async Task<JsonResult> GetAccountDetailsByPaymentorTagReference(string referencenumber)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();

                var validateResult = validator.ValidatePaymentorTagReference(referencenumber);
                if (validateResult.Valid)
                {
                    var accountActions = new AccountActions(_loggerActionAdapter, _leaseAPICallBuilder, _accountsApiCall, _accountsrepositoryserviceApiCall, _accessToken);

                    var accountdetails = await accountActions.GetAccountDetailsByParisorTagReference(referencenumber);


                    var json = Json(accountdetails);
                    json.StatusCode = Json(accountdetails).StatusCode;
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

        /// <summary>
        /// Verify Housing Account Details by Payment Reference and postcode.
        /// </summary>
        /// <param name="referencenumber">Payment reference number for Account</param>
        /// <returns>Returns account details in Json Object</returns>
        [Route("AccountDetailsByContactId")]
        [HttpGet]
        public async Task<JsonResult> GetAccountDetailsByContactId(string contactid)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();

                var validateResult = validator.ValidateContactId(contactid);
                if (validateResult.Valid)
                {
                    var accountActions = new AccountActions(_loggerActionAdapter,  _leaseAPICallBuilder, _accountsApiCall, _accountsrepositoryserviceApiCall,_accessToken);

                    var accountdetails = await accountActions.GetAccountDetailsByContactId(contactid);


                    var json = Json(accountdetails);
                    json.StatusCode = Json(accountdetails).StatusCode;
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