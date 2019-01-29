using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class ETRAController : Controller
    {
        private readonly ILoggerAdapter<TRAController> _logger;
        private readonly IHackneyGetCRM365Token _getCRM365AccessToken;
        private IHackneyHousingAPICall _hackneyETRAAccountApi;
        private readonly IOptions<AppConfiguration> _appConfiguration;
        private readonly IHackneyHousingAPICallBuilder _ETRAAPICallBuilder;
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;
        private readonly ILoggerAdapter<ETRAMeetingsAction> _actionsLogger;
        private IETRAMeetingsAction _etraMeetingsAction;
        public ETRAController(IETRAMeetingsAction etraMeetingsAction, ILoggerAdapter<ETRAMeetingsAction> actionsLogger, ILoggerAdapter<TRAController> loggerAdapter, IOptions<URLConfiguration> config, IOptions<AppConfiguration> appConfig, IHackneyGetCRM365Token accessToken)
        {
            var serviceFactory = new HackneyAccountsServiceFactory();
            _hackneyETRAAccountApi = serviceFactory.build();
            _logger = loggerAdapter;
            _appConfiguration = appConfig;
            _actionsLogger = actionsLogger;
            _ETRAAPICallBuilder = new HackneyHousingAPICallBuilder(config, _apiBuilderLoggerAdapter);
            _getCRM365AccessToken = accessToken;
            _etraMeetingsAction = etraMeetingsAction;
        }
        /// <summary>
        /// Creates an ETRA meeting
        /// </summary>
        /// <returns>A JSON object for a successfully created ETRA meeting request</returns>
        /// <response code="201">A successfully created ETRA meeting request</response>
        [Route("CreateETRAMeetingOrIssue")]
        [HttpPost]
        public async Task<JsonResult> Post([FromBody] ETRAIssue etraMeeting)
        {
            try
            {
                    var createEtraMeeting = _etraMeetingsAction.CreateETRAMeeting(etraMeeting).Result;
                    var json = Json(createEtraMeeting);
                    json.ContentType = "application/json";
                    json.StatusCode = 201;
                    return json;
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
        /// Gets ETRA Issues by TRA ID or parent interaction. Used to retrieve issue for a TRA or for a specific ETRA meeting.
        /// </summary>
        /// <returns>A list of ETRA issue for a TRA or a specific ETRA meeting</returns>
        /// <response code="200">Successfully retrieved ETRA issues request</response>
        [Route("GetETRAIssues/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetETRAISsues(string id, bool retrieveETRAMeetingIssues)
        {

            try
            {
                var getETRAIssues = _etraMeetingsAction.GetETRAIssuesByTRAorETRAMeeting(id, retrieveETRAMeetingIssues);
                var json = Json(getETRAIssues);
                json.ContentType = "application/json";
                json.StatusCode = 200;
                return json;
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
    }
}