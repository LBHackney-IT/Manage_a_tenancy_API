using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;

namespace ManageATenancyAPI.Repository
{
    public interface IEstateRepository
    {
        Task<List<UhProperty>> GetEstates(IList<string> estateId);
        Task<List<UhProperty>> GetEstatesNotInList(IList<string> usedEstates);
    }
}
