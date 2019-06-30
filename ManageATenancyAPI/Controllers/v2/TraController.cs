﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.AspNetCore.Mvc;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;

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
            IMeetingClaims claims = null;
            if (Request.Headers.ContainsKey(_authorization))
            {
                var token = Request.Headers[_authorization].ToString();
                token = token.Replace("bearer ", "", true, CultureInfo.InvariantCulture);

                claims = _jwtService.GetMeetingIdClaims(token, Environment.GetEnvironmentVariable("HmacSecret"));
            }

            return claims;
        }
    }



    [Produces("application/json")]
    [Route("v1/tra")]
    public class TRAController : BaseClaimsController
    {
        private readonly ISaveEtraMeetingUseCase _saveEtraMeetingUseCase;
        private readonly IGetEtraMeetingUseCase _getEtraMeetingUseCase;


        public TRAController(IJWTService jwtService, ISaveEtraMeetingUseCase saveEtraMeetingUseCase, IGetEtraMeetingUseCase getEtraMeetingUseCase): base(jwtService)
        {
            _saveEtraMeetingUseCase = saveEtraMeetingUseCase;
            _getEtraMeetingUseCase = getEtraMeetingUseCase;
        }

        /// <summary>
        /// Creates an ETRA meeting
        /// If the signoff object is provided then it will complete the meeting in one go
        /// If the signoff object is null then it will send out an email to the TRA representative stored in the database
        /// with a link to signoff the meeting with their name.
        /// In the second flow please post the signoff object via the Patch HttpMethod to complete the meeting
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
        [HttpGet]
        [ProducesResponseType(typeof(GetEtraMeetingOutputModel), 200)]
        [ProducesResponseType(typeof(UnauthorizedResult), 401)]
        public async Task<IActionResult> Get()
        {
            var claims = GetMeetingClaims();
            if (claims == null)
                return Unauthorized();

            var inputModel = new GetEtraMeetingInputModel
            {
                MeetingId = claims.MeetingId
            };

            var outputModel = await _getEtraMeetingUseCase.ExecuteAsync(inputModel, Request.GetCancellationToken()).ConfigureAwait(false);
            return Ok(outputModel);
        }

        /// <summary>
        /// Gets an ETRA meeting
        /// </summary>
        /// <returns>A JSON object for a successfully created ETRA meeting request</returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetEtraMeetingOutputModel), 200)]
        [ProducesResponseType(typeof(UnauthorizedResult), 401)]
        public async Task<IActionResult> Patch()
        {
            var claims = GetMeetingClaims();
            if (claims == null)
                return Unauthorized();

            var inputModel = new GetEtraMeetingInputModel
            {
                MeetingId = claims.MeetingId
            };

            var outputModel = await _getEtraMeetingUseCase.ExecuteAsync(inputModel, Request.GetCancellationToken()).ConfigureAwait(false);
            return Ok(outputModel);
        }
    }
}