using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public interface IEstateAction
    {
        Task<List<Estate>> GetEstates(IList<string> estateId);
        Task<List<Estate>> GetEstatesNotInList(IList<string> usedEstates);
    }
}
