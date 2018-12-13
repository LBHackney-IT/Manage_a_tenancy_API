using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public interface ITraEstatesAction
    {

        IList<TraEstate> GetEstatesByTraId(int traId);

        IList<string> GetAllUsedEstateRefs();
        bool AreUnusedEstates(List<string> traEsatateRefs);
        void AddEstateToTra(int traId, string estateId, string estateName);

    }
}
