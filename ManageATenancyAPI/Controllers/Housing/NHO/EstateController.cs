using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Authorize]
    [Route("v1/[controller]")]
    public class EstateController : Controller
    {
        private IEstateAction _estateAction;
        private IBlockAction _blockAction;
        private ITraEstateAction _traEstatesAction;
        public EstateController(IEstateAction estateAction, IBlockAction blockAction, ITraEstateAction traEstatesAction)
        {
            _estateAction = estateAction;
            _traEstatesAction = traEstatesAction;
            _blockAction = blockAction;
        }

        /// <summary>
        /// Location of Issue - list
        /// </summary>
        /// <param name="traId"></param>
        /// <returns></returns>
        [Route("estate/tra/{traId}")]
        [HttpGet]
        public async Task<HackneyResult<List<Estate>>> GetEstatesByTra(int traId)
        {
            IList<TraEstate> traEstates = _traEstatesAction.GetEstatesByTraId(traId);

            if (traEstates.Count == 0)
            {
                return HackneyResult<List<Estate>>.Create(new List<Estate>());
            }

            var estates = await _estateAction.GetEstates(traEstates.Select(x => x.EstateUHRef).ToList());
            foreach (var estate in estates)
            {
                estate.Blocks = await _blockAction.GetBlocksByEstateId(estate.EstateId);
            }
            return HackneyResult<List<Estate>>.Create(estates);
        }

        [Route("estate/patch/{patchId}/unassigned")]
        [HttpGet]
        public async Task<HackneyResult<List<Estate>>> GetUnassigned()
        {
            var usedEstates = _traEstatesAction.GetAllUsedEstateRefs();
            var estates = await _estateAction.GetEstatesNotInList(usedEstates);
            return HackneyResult<List<Estate>>.Create(estates);
        }

        [Route("tra/{traId}/estate")]
        [HttpPost]
        public async Task<IActionResult> AddEstateToTra([FromBody]AddEstateToTraRequest request)
        {
            var estateName = string.Empty;
            var estates = await _estateAction.GetEstates(new List<string>() { request.EstateId });
            if (estates.Count == 1)
            {
                estateName = estates.First().EstateName;
            }
            _traEstatesAction.AddEstateToTra(request.TraId, request.EstateId, estateName);
            return Ok();
        }

    }
}
