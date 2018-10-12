using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Factories;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Validators;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class TenancyManagementInteractionsController : Controller
    {
        private readonly ILoggerAdapter<TenancyManagementInteractionsController> _logger;
       
        private IHackneyHousingAPICall _hackneyLeaseAccountApi;
     
        private readonly IHackneyGetCRM365Token _getCRM365AccessToken;
        private readonly IHackneyHousingAPICallBuilder _leaseAPICallBuilder;
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;
        private readonly ILoggerAdapter<HackneyGetCRM365Token> _getCRM365AccessTokenLoggerAdapter;
        private readonly ILoggerAdapter<TenancyManagementActions> _loggerActionAdapter;
        private readonly IOptions<AppConfiguration> _appConfiguration;


        public TenancyManagementInteractionsController(ILoggerAdapter<TenancyManagementInteractionsController> loggerAdapter, ILoggerAdapter<TenancyManagementActions> loggerActionAdapter, IOptions<URLConfiguration> config, IOptions<AppConfiguration> appConfig)
        {
        
           var serviceFactory = new HackneyAccountsServiceFactory();
            _hackneyLeaseAccountApi = serviceFactory.build();
            _logger = loggerAdapter;
            _loggerActionAdapter = loggerActionAdapter;
            _appConfiguration = appConfig;
            _getCRM365AccessToken = new HackneyGetCRM365Token(config);
            _leaseAPICallBuilder = new HackneyHousingAPICallBuilder(config, _apiBuilderLoggerAdapter);

        }

        /// <summary>
        /// Creates a TenancyManagement request
        /// </summary>
        /// <param name="ContactId">ContactId</param>
        /// <param name="EnquirySubject">EnquirySubject</param>
        /// <param name="EstateOfficerId">EstateOfficerId</param>
        /// <param name="Subject">Subject</param>
        /// <param name="AdviceGiven">AdviceGiven</param>
        /// <param name="EstateOffice">EstateOffice</param>
        /// <param name="Source">Source</param>
        /// <param name="NatureofEnquiry">NatureofEnquiry</param>
        /// <param name="CRMServiceRequest">ServiceRequest</param>
        /// <returns>A JSON object for a successfully created TenancyManagement request</returns>
        /// <response code="200">A successfully created repair request</response>
        [Route("CreateTenancyManagementInteraction")]
        [HttpPost]
        public async Task<JsonResult> Post([FromBody] TenancyManagement interaction)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();

                var validationResult = validator.ValidateTenancyManagementinteractionrequest(interaction);

                if (validationResult.Valid)
                {
                    var TenancyManagementActions = new TenancyManagementActions(_loggerActionAdapter,
                      _leaseAPICallBuilder, _hackneyLeaseAccountApi, _getCRM365AccessToken, _appConfiguration);
                    var TenancyManagement =
                        await TenancyManagementActions.CreateTenancyManagementInteraction(interaction);
                    var json = Json(TenancyManagement);
                    json.StatusCode = Json(TenancyManagement).StatusCode;
                    json.ContentType = "application/json";
                    json.StatusCode = 201;
                    return json;
                }
                else
                {
                    var errors = validationResult.ErrorMessages.Select(error => new ApiErrorMessage
                    {
                        developerMessage = error,
                        userMessage = error
                    }).ToList();
                    var jsonResponse = Json(errors);
                    jsonResponse.StatusCode = 400;
                    return jsonResponse;
                }
               
            }
            catch (Exception e)
            {
                var errorMessage = new ApiErrorMessage
                {
                    developerMessage = e.Message,
                    userMessage = "We had some problems processing your request"
                };
                var jsonResponse = Json(errorMessage);
                jsonResponse.StatusCode = 500;
                return jsonResponse;
            }
        }

        /// <summary>
        /// Creates a Service request
        /// </summary>
        /// <param name="ContactId">ContactId</param>
        /// <param name="title">title</param>
        /// <param name="description">description</param>
        /// <param name="Subject">Subject</param>
        /// <param name="NatureofEnquiry">NatureofEnquiry</param>
        /// <returns>A JSON object for a successfully created TenancyManagement request</returns>
        /// <response code="200">A successfully created repair request</response>
        [Route("ServiceRequest")]
        [HttpPost]
        public async Task<JsonResult> ServiceRequest([FromBody] CRMServiceRequest serviceRequest)
        {
            try
            {
                var validator = new HousingQueryParameterValidator();

                var validationResult = validator.ValidateServicerequest(serviceRequest);

                if (validationResult.Valid)
                {
                    var TenancyManagementActions = new TenancyManagementActions(_loggerActionAdapter,
                       _leaseAPICallBuilder, _hackneyLeaseAccountApi, _getCRM365AccessToken, _appConfiguration);
                    var serviceRequestResponse =
                        await TenancyManagementActions.CreateCrmServiceRequest(serviceRequest);
                    var json = Json(serviceRequestResponse);
                    json.StatusCode = Json(serviceRequestResponse).StatusCode;
                    json.ContentType = "application/json";
                    json.StatusCode = 201;
                    return json;
                }
                else
                {

                    var errors = validationResult.ErrorMessages.Select(error => new ApiErrorMessage
                    {
                        developerMessage = error,
                        userMessage = error
                    }).ToList();
                    var jsonResponse = Json(errors);
                    jsonResponse.StatusCode = 400;
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                var errorMessage = new ApiErrorMessage
                {
                    developerMessage = e.Message,
                    userMessage = "We had some problems processing your request"
                };
                var jsonResponse = Json(errorMessage);
                jsonResponse.StatusCode = 500;
                return jsonResponse;
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateTenancyManagementInteratction([FromBody] TenancyManagement tenancyInteraction)
        {
            try
            {

                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidatePatchTenancyManagementInteraction(tenancyInteraction);
                object TenancyManagement = null;
                var TenancyManagementActions = new TenancyManagementActions(_loggerActionAdapter, _leaseAPICallBuilder, _hackneyLeaseAccountApi, _getCRM365AccessToken, _appConfiguration);

                if (validateResult.Valid)
                {
                    TenancyManagement = await TenancyManagementActions.UpdateTenancyManagementInteraction(tenancyInteraction);

                    var json = Json(TenancyManagement);
                    json.StatusCode = Json(TenancyManagement).StatusCode;
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
            catch (Exception e)
            {
                var errorMessage = new ApiErrorMessage
                {
                    developerMessage = e.Message,
                    userMessage = "We had some problems processing your request"
                };
                var jsonResponse = Json(errorMessage);
                jsonResponse.StatusCode = 500;
                jsonResponse.ContentType = "application/json";
                return jsonResponse;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(string contactId, string personType)
        {
            try
            {
                var TenancyManagementActions = new TenancyManagementActions(_loggerActionAdapter,_leaseAPICallBuilder, _hackneyLeaseAccountApi, _getCRM365AccessToken, _appConfiguration);
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidateGetTenancyManagementInteraction(contactId, personType);

                object TenancyManagement = null;
                if (validateResult.Valid)
                {
                    TenancyManagement = await TenancyManagementActions.GetTenancyManagementInteraction(contactId, personType);
                    var json = Json(TenancyManagement);
                    json.StatusCode = Json(TenancyManagement).StatusCode;
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
            catch (Exception ex)
            {
                var errorMessage = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = "We had some problems processing your request"
                };
                var jsonResponse = Json(errorMessage);
                jsonResponse.StatusCode = 500;
                jsonResponse.ContentType = "application/json";
                return jsonResponse;
            }
        }

        /// <summary>
        /// Get group Tray 
        /// </summary>
        /// <param name="OfficeId">OfficeId of the Group Tray</param>
        /// <returns>A JSON object for a successfully returns for Group Tray TenancyManagement request</returns>
        [Route("GetAreaTrayIneractions")]
        [HttpGet]
        public async Task<IActionResult> GetAreaTrayIneractions(string OfficeId)
        {
            try
            {
                var TenancyManagementActions = new TenancyManagementActions(_loggerActionAdapter,  _leaseAPICallBuilder, _hackneyLeaseAccountApi, _getCRM365AccessToken, _appConfiguration);
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidateGetAreaTrayIneractions(OfficeId);

                object TenancyManagement = null;
                if (validateResult.Valid)
                {
                    TenancyManagement = await TenancyManagementActions.GetAreaTrayInteractions(OfficeId);
                    var json = Json(TenancyManagement);
                    json.StatusCode = Json(TenancyManagement).StatusCode;
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
            catch (Exception ex)
            {
                var errorMessage = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = "We had some problems processing your request"
                };
                var jsonResponse = Json(errorMessage);
                jsonResponse.StatusCode = 500;
                jsonResponse.ContentType = "application/json";
                return jsonResponse;
            }
        }

        /// <summary>
        /// TransferCallToAreaAndPatch
        /// </summary>
        /// <param name="tenancyInteraction"> List Of interaction or call to be updated/tranferred </param>
        /// <returns>List Of interaction or call which has been successfully updated/tranferred</returns>
        [Route("TransferCall")]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] TenancyManagement tenancyInteraction)
        {
            var TenancyManagementActions = new TenancyManagementActions(_loggerActionAdapter, _leaseAPICallBuilder, _hackneyLeaseAccountApi, _getCRM365AccessToken, _appConfiguration);
            object TenancyManagement = null;
            try
            {
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidateTransferCallToPatchAndArea(tenancyInteraction);
                if (validateResult.Valid)
                {
                    TenancyManagement = await TenancyManagementActions.TransferCallToAreaAndPatch(tenancyInteraction);
                    var json = Json(TenancyManagement);
                    json.StatusCode = Json(TenancyManagement).StatusCode;
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
            catch (Exception ex)
            {
                var errorMessage = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = "We had some problems processing your request"
                };
                var jsonResponse = Json(errorMessage);
                jsonResponse.StatusCode = 500;
                jsonResponse.ContentType = "application/json";
                return jsonResponse;
            }

        }

    }
}