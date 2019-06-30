using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ManageATenancyAPI.Services.JWT;

namespace ManageATenancyAPI.Controllers.v2
{

    public class BaseClaimsController : Controller
    {
        private readonly IJWTService _jwtService;
        private string _authorization;
        protected IManageATenancyClaims Claims { get; set; }
        public BaseClaimsController(IJWTService jwtService)
        {
            _jwtService = jwtService;
            _authorization = "Authorization";
        }

        protected IManageATenancyClaims GetClaims()
        {
            if (Request.Headers.ContainsKey(_authorization))
            {
                var authString = Request.Headers[_authorization].ToString();
                authString = authString.Replace("bearer ", "", true, CultureInfo.InvariantCulture);

                Claims = _jwtService.GetManageATenancyClaims(authString, Environment.GetEnvironmentVariable("HmacSecret"));
            }

            return Claims;
        }

        protected string CreateManageATenancySingleMeetingToken(Guid traMeetingId)
        {
            string jwtToken = string.Empty;

            jwtToken = _jwtService.CreateManageATenancySingleMeetingToken(traMeetingId, Environment.GetEnvironmentVariable("HmacSecret"));

            return jwtToken;
        }

    }



    [Produces("application/json")]
    [Route("v1/tra")]
    public class TRAController : BaseClaimsController
    {
        private readonly ISaveEtraMeetingUseCase _saveEtraMeetingUseCase;

        public TRAController(IJWTService jwtService,  ISaveEtraMeetingUseCase saveEtraMeetingUseCase) : base(jwtService)
        {
            _saveEtraMeetingUseCase = saveEtraMeetingUseCase;
        }

        /// <summary>
        /// Creates an ETRA meeting
        /// </summary>
        /// <returns>A JSON object for a successfully created ETRA meeting request</returns>
        /// <response code="201">A successfully created ETRA meeting request</response>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]SaveETRAMeetingInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var claims = GetClaims();

            var outputModel = await _saveEtraMeetingUseCase.ExecuteAsync(inputModel, claims, Request.GetCancellationToken()).ConfigureAwait(false);
            return Ok(outputModel);
        }

        [HttpGet]
        [Route("GetTokenForSingleMeeting/{traMeetingId}")]
        public async Task<IActionResult> GetTokenForSingleMeeting(string traMeetingId)
        {
            Guid validMeetingId;

            bool isValid = Guid.TryParse(traMeetingId, out validMeetingId);
            if(isValid)
            {
                var meetingToken = CreateManageATenancySingleMeetingToken(validMeetingId);
                return Ok(meetingToken);
            }
            else
            {
                return BadRequest(traMeetingId);
            }
        }
    }
}