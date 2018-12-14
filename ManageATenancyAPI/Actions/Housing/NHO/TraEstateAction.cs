using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;
using ManageATenancyAPI.Repository;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class TraEstateAction : ITraEstateAction
    {
        private ITraEstateRepository _traEstateRepository;
        public TraEstateAction(ITraEstateRepository traEstateRepository)
        {
            _traEstateRepository = traEstateRepository;
        }

        public IList<TraEstate> GetEstatesByTraId(int traId)
        {
            return _traEstateRepository.GetEstatesByTraId(traId);
        }

        public IList<string> GetAllUsedEstateRefs()
        {
            return _traEstateRepository.GetAllUsedEstateRefs();
        }

        public void AddEstateToTra(int traId, string estateId, string estateName)
        {
             _traEstateRepository.AddEstateToTra(traId, estateId, estateName);
        }


        public bool AreUnusedEstates(List<string> traEsatateRefs)
        {
            return (traEsatateRefs.Intersect(_traEstateRepository.GetAllUsedEstateRefs()).Any());
        }

    }
}
