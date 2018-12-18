using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public interface IBlockAction
    {
        Task<IEnumerable<Block>> GetBlocksByEstateId(string estateId);
    }
}
