using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;
using ManageATenancyAPI.Repository;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class BlockAction : IBlockAction
    {
        private IBlockRepository _blockRepository;
        public BlockAction(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }

        public async Task<IEnumerable<Block>> GetBlocksByEstateId(string estateId)
        {


            var results = new List<Block>();
            foreach (var uhProperty in await _blockRepository.GetBlocksByEstateId(estateId))
            {
                results.Add(Block.FromModel(uhProperty));
            }
            return results;
        }
    }
}
