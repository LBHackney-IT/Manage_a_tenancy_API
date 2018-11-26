using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Route("v1/[controller]")]
    public class EstateController : Controller
    {
        private IEstateRepository _estateRepository;
        private IBlockRepository _blockRepository;
        private ITraEstatesRepository _traEstatesRepository;
        public EstateController(IEstateRepository estateRepository, IBlockRepository blockRepository, ITraEstatesRepository traEstatesRepository)
        {
            _estateRepository = estateRepository;
            _traEstatesRepository = traEstatesRepository;
            _blockRepository = blockRepository;
        }

        [Route("/estate/tra/{traId}")]
        [HttpGet]
        public async Task<HackneyResult<List<Estate>>> GetEstatesByTra(int traId)
        {
            IList<TraEstate> traEstates = _traEstatesRepository.GetEstatesByTraId(traId);

            if (traEstates.Count == 0)
            {
                return HackneyResult<List<Estate>>.Create(new List<Estate>());
            }

            var estates = await _estateRepository.GetEstates(traEstates.Select(x => x.EstateUHRef).ToList());
            foreach (var estate in estates)
            {
                estate.Blocks = await _blockRepository.GetBlocksByEstateId(estate.EstateId);
            }
            return HackneyResult<List<Estate>>.Create(estates);
        }

        [Route("/estate/patch/{patchId}/unassigned")]
        [HttpGet]
        public async Task<HackneyResult<List<Estate>>> GetUnassigned()
        {
            var usedEstates = _traEstatesRepository.GetAllUsedEstateRefs();
            var estates = await _estateRepository.GetEstatesNotInList(usedEstates);
            return HackneyResult<List<Estate>>.Create(estates);
        }
    }
}
