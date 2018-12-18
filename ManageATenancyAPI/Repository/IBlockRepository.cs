using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;

namespace ManageATenancyAPI.Repository
{
    public interface IBlockRepository
    {
        Task<IEnumerable<UhProperty>> GetBlocksByEstateId(string estateId);
    }
}
