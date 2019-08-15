using System;
using System.Threading.Tasks;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.v2.Tra
{
    [Authorize]
    [ApiVersion("2.0")]
    [Produces("application/json")]
    [Route("v2/tra/meeting/issues")]
    public class TRAIssuesController : BaseClaimsController
    {
        private readonly IEscalateIssuesUseCase _escalateIssuesUseCase;

        public TRAIssuesController(IJWTService jwtService, IEscalateIssuesUseCase escalateIssuesUseCase) : base(jwtService)
        {
            _escalateIssuesUseCase = escalateIssuesUseCase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(EscalateIssuesOutputModel), 200)]
        [ProducesResponseType(typeof(UnauthorizedResult), 401)]
        public async Task<IActionResult> Post()
        {
            var outputModel = await _escalateIssuesUseCase.ExecuteAsync(Request.GetCancellationToken()).ConfigureAwait(false);
            return Ok(outputModel);
        }

    }
}