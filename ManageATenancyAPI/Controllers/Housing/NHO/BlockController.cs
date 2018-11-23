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
        private IBlockRepository _estateBlockRepository;

        public BlockController(IBlockRepository estateBlockRepository)
        {
            _estateBlockRepository = estateBlockRepository;

        }

        [Route("block/estate/{estateId}")]
        [HttpGet]
        public async Task<HackneyResult<IEnumerable<Block>>> GetBlocksByEstate(string estateId)
        {
            var estateBlocks = await _estateBlockRepository.GetBlocksByEstateId(estateId);
            return HackneyResult<IEnumerable<Block>>.Create(estateBlocks);
        }
    }
}
