using System.Threading.Tasks;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.AspNetCore.Mvc;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace ManageATenancyAPI.Controllers.v2
{
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Route("v2/tra/meeting")]
    [EnableCors("AllowAny")]
    
    public class TRAController : BaseClaimsController
    {
        private readonly ISaveEtraMeetingUseCase _saveEtraMeetingUseCase;
        private readonly IGetEtraMeetingUseCase _getEtraMeetingUseCase;
        private readonly ISignOffMeetingUseCase _signOffEtraMeetingUseCase;


        public TRAController(IJWTService jwtService, ISaveEtraMeetingUseCase saveEtraMeetingUseCase, IGetEtraMeetingUseCase getEtraMeetingUseCase, ISignOffMeetingUseCase signOffEtraMeetingUseCase): base(jwtService)
        {
            _saveEtraMeetingUseCase = saveEtraMeetingUseCase;
            _getEtraMeetingUseCase = getEtraMeetingUseCase;
            _signOffEtraMeetingUseCase = signOffEtraMeetingUseCase;
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
        [ProducesResponseType(typeof(SaveEtraMeetingOutputModel), 200)]
        [ProducesResponseType(typeof(BadRequestResult), 400)]
        [Authorize]
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
        [Authorize]
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
        [HttpPatch]
        [ProducesResponseType(typeof(SignOffMeetingOutputModel), 200)]
        [ProducesResponseType(typeof(UnauthorizedResult), 401)]
        [Authorize]
        public async Task<IActionResult> Patch([FromBody]SignOffMeetingInputModel inputModel)
        {
            var claims = GetMeetingClaims();
            if (claims == null)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var outputModel = await _signOffEtraMeetingUseCase.ExecuteAsync(inputModel, Request.GetCancellationToken()).ConfigureAwait(false);
            return Ok(outputModel);
        }
    }
}