using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;

namespace ManageATenancyAPI.Repository
{
    public interface ITraEstateRepository
    {
        IList<TraEstate> GetEstatesByTraId(int traId);

        IList<string> GetAllUsedEstateRefs();
        void AddEstateToTra(int traId, string estateId,string estateName);
    }
}
