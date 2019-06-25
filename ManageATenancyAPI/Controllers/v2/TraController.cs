using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Database.Models;
using ManageATenancyAPI.Gateways.GetAllTRAs;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using ManageATenancyAPI.UseCases.TRA.GetAllTRAs;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.v2
{
    [Produces("application/json")]
    [Route("v1/tra")]
    public class TRAController : Controller
    {
        private readonly IGetAllTRAsUseCase _getAllTrAsUseCase;
        private readonly ISaveEtraMeetingUseCase _saveEtraMeetingUseCase;

        public TRAController(IGetAllTRAsUseCase getAllTrAsUseCase, ISaveEtraMeetingUseCase saveEtraMeetingUseCase)
        {
            _getAllTrAsUseCase = getAllTrAsUseCase;
            _saveEtraMeetingUseCase = saveEtraMeetingUseCase;
        }

        /// <summary>
        /// Creates an ETRA meeting
        /// </summary>
        /// <returns>A JSON object for a successfully created ETRA meeting request</returns>
        /// <response code="201">A successfully created ETRA meeting request</response>
        [HttpGet]
        public async Task<GetAllTRAsOutputModel> Get()
        {
            var outputModel = await _getAllTrAsUseCase.ExecuteAsync(Request.GetCancellationToken()).ConfigureAwait(false);
            return outputModel;
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

            var outputModel = await _saveEtraMeetingUseCase.ExecuteAsync(inputModel, Request.GetCancellationToken()).ConfigureAwait(false);
            return Ok(outputModel);
        }
    }

    public class GetAllTRAsOutputModel
    {
        public IList<TRA> TRAs { get; set; }
    }
}