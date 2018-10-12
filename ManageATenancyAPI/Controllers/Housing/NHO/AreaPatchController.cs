using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Factories;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using Microsoft.AspNetCore.Mvc;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Validators;
using ManageATenancyAPI.Formatters;
using ManageATenancyAPI.Models.Housing.NHO;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;


namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class AreaPatchController : Controller
    {
        private readonly ILoggerAdapter<AreaPatchController> _logger;
        private readonly ILoggerAdapter<AreaPatchActions> _loggerActionAdapter;
        private readonly IHackneyGetCRM365Token _getCRM365AccessToken;
        private readonly HackneyHousingAPICallBuilder _areaPatchAPICallBuilder;
        private IPostcodeValidator _postCodeValidator;       
        private IHackneyHousingAPICall _areaPatchApiCall;
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;

        
        
        public AreaPatchController(ILoggerAdapter<AreaPatchController> loggerAdapter, ILoggerAdapter<AreaPatchActions> loggerActionAdapter, IOptions<URLConfiguration> config)
        {
            var serviceFactory = new HackneyAccountsServiceFactory();
            _areaPatchApiCall = serviceFactory.build();
            _logger = loggerAdapter;
            _loggerActionAdapter = loggerActionAdapter;
            _getCRM365AccessToken = new HackneyGetCRM365Token(config);
            _areaPatchAPICallBuilder = new HackneyHousingAPICallBuilder(config, _apiBuilderLoggerAdapter);
            _postCodeValidator = new PostcodeValidator();           
        }

        [Route("GetAreaPatch")]
        [HttpGet]
        public async Task<JsonResult> GetAreaPatch(string postcode, string UPRN)
        {
            try
            {                
                if (_postCodeValidator.Validate(postcode) && !string.IsNullOrEmpty(UPRN))
                {                    
                    var areaPatchActionRequest = new AreaPatchActions(_loggerActionAdapter,_areaPatchAPICallBuilder, _areaPatchApiCall,_getCRM365AccessToken);

                    var areaPatchActionResult = await areaPatchActionRequest.GetAreaPatch(new PostcodeFormatter().FormatPostcode(postcode), UPRN);

                    var json = Json(areaPatchActionResult);
                    json.StatusCode = Json(areaPatchActionResult).StatusCode;
                    json.ContentType = "application/json";
                    return json;

                    
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = " Bad Request",
                            userMessage = "The request isn't right; please verify the parameters"
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

        [Route("GetAllOfficersPerArea")]
        [HttpGet]
        public async Task<JsonResult> GetAllOfficersPerArea(string areaId)
        {
            try
            {
                var areaPatchActionRequest = new AreaPatchActions(_loggerActionAdapter, _areaPatchAPICallBuilder, _areaPatchApiCall,_getCRM365AccessToken);
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidateGetAllOfficersPerArea(areaId);

                object officerList = null;
                if (validateResult.Valid)
                {
                    officerList = await areaPatchActionRequest.GetAllOfficersPerArea(areaId);
                    var json = Json(officerList);
                    json.StatusCode = Json(officerList).StatusCode;
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


        [Route("UpdateOfficerPatchOrAreaManager")]
        [HttpPut]
        public async Task<JsonResult> UpdateOfficerPatch([FromBody] OfficerAreaPatch officerAreaPatch)
        {
            try
            {
              
                var validator = new HousingQueryParameterValidator();
                var validateResult = validator.ValidateUpdateOfficerPatchOrAreaManager(officerAreaPatch);

                if (validateResult.Valid)
                {
                    var areaPatchActionRequest = new AreaPatchActions(_loggerActionAdapter, _areaPatchAPICallBuilder, _areaPatchApiCall,_getCRM365AccessToken);
                    var updatePatch = await areaPatchActionRequest.UpdatePatchOrManager(officerAreaPatch);
                    var json = Json(updatePatch);
                    json.StatusCode = 201;
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

        [Route("GetAllUnassignedOfficers")]
        [HttpGet]
        public async Task<JsonResult> GetAllUnassignedOfficers()
        {
            try
            {

                var areaPatchActionRequest = new AreaPatchActions(_loggerActionAdapter, _areaPatchAPICallBuilder, _areaPatchApiCall,_getCRM365AccessToken);
                var officerList = await areaPatchActionRequest.GetAllOfficersThatAreNotAssignedToAPatchOrArea();
                    var json = Json(officerList);
                    json.StatusCode = Json(officerList).StatusCode;
                    json.ContentType = "application/json";
                    return json;
         
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
    
