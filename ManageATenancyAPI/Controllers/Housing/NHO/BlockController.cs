using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Route("v1/[controller]")]
    public class BlockController : Controller
    {
        private IEstateBlockRepository _estateBlockRepository;

        public BlockController(IEstateBlockRepository estateBlockRepository)
        {
            _estateBlockRepository = estateBlockRepository;

        }

        [Route("block/estate/{estateId}")]
        [HttpGet]
        public async Task<HackneyResult<IEnumerable<EstateBlock>>> GetBlocksbyEstate(int estateId)
        {
            var estateBlocks = await _estateBlockRepository.GetBlocksByEstateId(estateId);
            return HackneyResult<IEnumerable<EstateBlock>>.Create(estateBlocks);
        }
    }
}
