using System;
using System.Globalization;
using System.Threading.Tasks;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.AspNetCore.Mvc;
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

        protected IManageATenancyClaims GetHousingOfficerClaims()
        {
            if (Request.Headers.ContainsKey(_authorization))
            {
                var authString = Request.Headers[_authorization].ToString();
                authString = authString.Replace("bearer ", "", true, CultureInfo.InvariantCulture);

                Claims = _jwtService.GetManageATenancyClaims(authString, Environment.GetEnvironmentVariable("HmacSecret"));
            }

            return Claims;
        }

        protected IMeetingClaims GetMeetingClaims()
        {
            if (Request.Headers.ContainsKey(_authorization))
            {
                var authString = Request.Headers[_authorization].ToString();
                authString = authString.Replace("bearer ", "", true, CultureInfo.InvariantCulture);

                 = _jwtService.GetManageATenancyClaims(authString, Environment.GetEnvironmentVariable("HmacSecret"));
            }

            return Claims;
        }
    }

    [Produces("application/json")]
    [Route("v1/tra")]
    public class TRAController : BaseClaimsController
    {
        private readonly ISaveEtraMeetingUseCase _saveEtraMeetingUseCase;

        public TRAController(IJWTService jwtService, ISaveEtraMeetingUseCase saveEtraMeetingUseCase): base(jwtService)
        {
            _saveEtraMeetingUseCase = saveEtraMeetingUseCase;
        }

        /// <summary>
        /// Creates an ETRA meeting
        /// </summary>
        /// <returns>A JSON object for a successfully created ETRA meeting request</returns>
        /// <response code="200">A successfully created ETRA meeting request</response>
        [HttpPost]
        [ProducesResponseType(typeof(SaveEtraMeetingOutputModelOutputModel), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        public async Task<IActionResult> Post([FromBody]SaveETRAMeetingInputModel inputModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var claims = GetHousingOfficerClaims();

            var outputModel = await _saveEtraMeetingUseCase.ExecuteAsync(inputModel, claims, Request.GetCancellationToken()).ConfigureAwait(false);
            return Ok(outputModel);
        }

        /// <summary>
        /// Gets an ETRA meeting
        /// </summary>
        /// <returns>A JSON object for a successfully created ETRA meeting request</returns>
        /// <response code="201">A successfully created ETRA meeting request</response>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var claims = GetHousingOfficerClaims();

            var inputModel = new SaveETRAMeetingInputModel();

            var outputModel = await _.ExecuteAsync(inputModel, claims, Request.GetCancellationToken()).ConfigureAwait(false);
            return Ok(outputModel);
        }
    }
}