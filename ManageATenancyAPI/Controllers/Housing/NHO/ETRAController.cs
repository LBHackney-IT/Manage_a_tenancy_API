using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Factories.Housing;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Services;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Authorize]
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class ETRAController : Controller
    {
        private readonly ILoggerAdapter<TRAController> _logger;
        private readonly IHackneyGetCRM365Token _getCRM365AccessToken;
        private readonly IHackneyHousingAPICall _hackneyETRAAccountApi;
        private readonly IOptions<AppConfiguration> _appConfiguration;
        private readonly IHackneyHousingAPICallBuilder _ETRAAPICallBuilder;
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _apiBuilderLoggerAdapter;
private readonly ILoggerAdapter<ETRAMeetingsAction> _actionsLogger;
        private readonly IETRAMeetingsAction _etraMeetingsAction;
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
            catch (Exception ex)
            {
                var errorMessage = new ApiErrorMessage
                {
                    developerMessage = ex.Message,
                    userMessage = "We had some problems processing your request"
                };
                var jsonResponse = Json(errorMessage);
                jsonResponse.StatusCode = 500;
                return jsonResponse;
            }
        }


        [Route("UpdateETRAIssue")]
        [HttpPatch]
        public async Task<JsonResult> UpdateETRAIssue([FromBody] UpdateETRAIssue etraIssueToBeUpdated)
        {
            try
            {
                var createEtraMeeting = _etraMeetingsAction.UpdateIssue(etraIssueToBeUpdated).Result;
                var json = Json(createEtraMeeting);
                json.ContentType = "application/json";
                json.StatusCode = 201;
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
                return jsonResponse;
            }
        }

        /// <summary>
        /// Records the people who attended an ETRA meeting.
        /// </summary>
        /// <param name="id">Meeting id.</param>
        /// <param name="request">Object containing the councillors, other staff and total attendance of the meeting.</param>
        /// <returns>Whether the meeting attendance has been successfully recorded</returns>
        /// <response code="200">Successfully recorded meeting attendance</response>
        /// <response code="404">No meeting with the specified id found</response>
        /// <response code="400">Null or empty id or null request</response>
        [Route("record-attendance/{id}")]
        [HttpPatch]
        public async Task<ActionResult<RecordETRAMeetingAttendanceResponse>> RecordAttendance(string id, [FromBody] RecordETRAMeetingAttendanceRequest request)
        {
            if (string.IsNullOrEmpty(id) || request == null)
                return BadRequest();

            var meeting = await _etraMeetingsAction.GetMeeting(id);

            if (meeting == null)
                return NotFound();

            var response = await _etraMeetingsAction.RecordETRAMeetingAttendance(meeting.Id, request);
            return Ok(response);
        }

        /// <summary>
        /// Records a response from a service area to an ETRA issue.
        /// </summary>
        /// <param name="id">Issue id.</param>
        /// <param name="request">Object containing the required data to record the response.</param>
        /// <returns>Whether the issue response has been successfully recorded</returns>
        /// <response code="200">Successfully recorded issue response</response>
        /// <response code="400">Null or empty id or null request</response>
        [Route("add-issue-response/{id}")]
        [HttpPatch]
        public async Task<ActionResult<ETRAUpdateResponse>> AddIssueResponse(string id, [FromBody]ETRAIssueResponseRequest request)
        {
            if (string.IsNullOrEmpty(id) ||
                request == null ||
                (request.IssueStage == HackneyProcessStage.NotCompleted && !request.ProjectedCompletionDate.HasValue))
                return BadRequest();
            
            var response = await _etraMeetingsAction.AddETRAIssueResponse(id, request);
            return Ok(response);
        }

        /// <summary>
        /// Rejects the response to an issue from a service area.
        /// </summary>
        /// <param name="issueId">Issue id.</param>
        /// <param name="request">Object containing the required data to reject the response.</param>
        /// <returns>Whether the issue response has been successfully rejected</returns>
        /// <response code="200">Successfully recorded issue response</response>
        /// <response code="400">Null or empty id or null request</response>
        [Route("reject-response/{issueId}")]
        [HttpPatch]
        public async Task<ActionResult<ETRAUpdateResponse>> RejectResponse(string issueId, [FromBody]ETRAIssueRejectResponseRequest request)
        {
            if (string.IsNullOrEmpty(issueId) ||
                request == null)
                return BadRequest();

            var response = await _etraMeetingsAction.RejectETRAIssueResponse(issueId, request);
            return Ok(response);
        }

        /// <summary>
        /// Closes an incident in the CRM.
        /// </summary>
        /// <param name="id">The incident id.</param>
        /// <param name="note">An optional note to store when closing the incident.</param>
        /// <returns>Whether the incident has been successfully closed</returns>
        /// <response code="200">Successfully closed the incident</response>
        /// <response code="400">Empty id</response>
        [Route("close-incident/{id}")]
        [HttpPatch]
        public async Task<ActionResult<dynamic>> CloseIncident(Guid id, [FromBody]string note)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var response = await _etraMeetingsAction.CloseIncident(note, id);
            return Ok(response);
        }

        /// <summary>
        /// Finalises ETRA Meetings by meeting id, and optionally a signatory with their role.
        /// </summary>
        /// <param name="id">Meeting id.</param>
        /// <param name="request">(Optional) Object containing the guid reference of the signature and the string with the signatory's role.</param>
        /// <returns>Whether the meeting has been successfully finalised</returns>
        /// <response code="200">Successfully finalised meeting</response>
        /// <response code="404">No meeting with the specified id found</response>
        /// <response code="403">Meeting has already been finalised</response>
        [Route("finalise-meeting/{id}")]
        [HttpPatch]
        public async Task<IActionResult> FinaliseMeeting(string id, [FromBody] FinaliseETRAMeetingRequest request)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("id parameter cannot be null or empty", "id");

            var meeting = await _etraMeetingsAction.GetMeeting(id);

            if (meeting == null)
                return NotFound();

            if (meeting.ConfirmationDate != null)
                return Forbid();

            var response = await _etraMeetingsAction.FinaliseMeeting(meeting.Id, request);
            return Ok(HackneyResult<FinaliseETRAMeetingResponse>.Create(response));
        }

        /// <summary>
        /// Gets the details of an ETRA meeting.
        /// </summary>
        /// <param name="id">Meeting id.</param>
        /// <returns>The details of the meeting</returns>
        /// <response code="200">Successfully recorded issue response</response>
        /// <response code="400">Empty id</response>
        /// <response code="404">No such meeting exists</response>
        [Route("get-meeting-details/{id}")]
        [HttpGet]
        public async Task<ActionResult<ETRAMeeting>> GetMeetingDetails(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var meeting = await _etraMeetingsAction.GetMeeting(id.ToString());

            if (meeting == null)
                return NotFound();

            return Ok(meeting);
        }

        /// <summary>
        /// Gets ETRA Issues by TRA ID or parent interaction. Used to retrieve issue for a TRA or for a specific ETRA meeting.
        /// </summary>
        /// <param name="id">TRA Id for issues per TRA or Parent Interaction ID for issues per meeting</param>
        /// <param name="retrieveETRAMeetingIssues">True if meeting specific issues are to be retrieved. False if all issues per TRA are to be retrieved.</param>
        /// <returns>A list of ETRA issue for a TRA or a specific ETRA meeting</returns>
        /// <response code="200">Successfully retrieved ETRA issues request</response>
        [Route("GetETRAIssues/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetETRAISsues(string id, bool retrieveETRAMeetingIssues)
        {

            try
            {
                var getETRAIssues = _etraMeetingsAction.GetETRAIssuesByTRAorETRAMeeting(id, retrieveETRAMeetingIssues).Result;
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

        /// <summary>
        /// Gets all meetings for a specified TRA.
        /// </summary>
        /// <param name="id">TRA id.</param>
        /// <returns>A list of ETRA meetings</returns>
        /// <response code="200">Successfully returned meetings</response>
        /// <response code="400">Null or empty id</response>
        [Route("etra-meetings-by-tra/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ETRAMeeting>>> GetETRAMeetingsByTRA(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var meetings = await _etraMeetingsAction.GetETRAMeetingsForTRAId(id);

            return Ok(meetings);
        }
    }
}