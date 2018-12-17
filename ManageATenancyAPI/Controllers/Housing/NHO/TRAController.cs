using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Formatters;
using ManageATenancyAPI.Helpers.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class TRAController : Controller
    {

        private readonly ILoggerAdapter<TRAController> _logger;
        private ITRARepository _traRepository;
        private readonly ILoggerAdapter<TRAActions> _actionsLogger;
        public TRAController(ILoggerAdapter<TRAActions> actionsLogger ,ILoggerAdapter<TRAController> loggerAdapter, IOptions<URLConfiguration> config, ITRARepository traRepository)
        {
            _logger = loggerAdapter;
            _actionsLogger = actionsLogger;
            _traRepository = traRepository;
        }

        [Route("GetTRAForPatch")]
        [HttpGet]
        public async Task<JsonResult> GetTRAbyPatchId(Guid patchId)
        {
            try
            {
                if (patchId != Guid.Empty)
                {
                    var actions = new TRAActions(_actionsLogger, _traRepository);
                    var results = actions.GetTRAForPatch(patchId.ToString()).Result;
                    return Json(results);
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Bad Request",
                            userMessage = "Please provide a valid patch ID"
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
                        userMessage = "We had some problems processing your request - " + ex.InnerException
                    }
                };
                _logger.LogError(ex.Message);
                var json = Json(errors);
                json.StatusCode = 500;
                json.ContentType = "application/json";
                return json;
            }
        }

        [Route("GetTRAInformation")]
        [HttpGet]
        public async Task<JsonResult> GetTRAInformation(int TRAId)
        {
            try
            {
                if (TRAId != 0)
                {
                    var actions = new TRAActions(_actionsLogger, _traRepository);
                    var results = actions.GetTRAInformation(TRAId).Result;
                    return Json(results);
                }
                else
                {
                    var errors = new List<ApiErrorMessage>
                    {
                        new ApiErrorMessage
                        {
                            developerMessage = "Bad Request",
                            userMessage = "Please provide a valid TRA ID"
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
                        userMessage = "We had some problems processing your request - " + ex.InnerException
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