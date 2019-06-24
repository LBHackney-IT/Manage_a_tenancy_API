using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Database.Models;
using ManageATenancyAPI.Gateways.GetAllTRAs;
using ManageATenancyAPI.Helpers;
using ManageATenancyAPI.UseCases.TRA.GetAllTRAs;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.v2
{
    [Produces("application/json")]
    [Route("v1/tra")]
    public class TRAController : Controller
    {
        private readonly IGetAllTRAsUseCase _getAllTrAsUseCase;

        public TRAController(IGetAllTRAsUseCase getAllTrAsUseCase)
        {
            _getAllTrAsUseCase = getAllTrAsUseCase;
        }

        /// <summary>
        /// Creates an ETRA meeting
        /// </summary>
        /// <returns>A JSON object for a successfully created ETRA meeting request</returns>
        /// <response code="201">A successfully created ETRA meeting request</response>
        [HttpPost]
        public async Task<GetAllTRAsOutputModel> Get()
        {
            var outputModel = await _getAllTrAsUseCase.ExecuteAsync(Request.GetCancellationToken()).ConfigureAwait(false);
            return outputModel;
        }
    }

    public class GetAllTRAsOutputModel
    {
        public IList<TRA> TRAs { get; set; }
    }
}