using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;
using ManageATenancyAPI.Repository;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class TraEstatesesAction : ITraEstatesAction
    {
        private ITraEstatesRepository _traEstatesRepository;
        public TraEstatesesAction(ITraEstatesRepository traEstatesRepository)
        {
            _traEstatesRepository = traEstatesRepository;
        }

        public IList<TraEstate> GetEstatesByTraId(int traId)
        {
            return _traEstatesRepository.GetEstatesByTraId(traId);
        }

        public IList<string> GetAllUsedEstateRefs()
        {
            return _traEstatesRepository.GetAllUsedEstateRefs();
        }

        public void AddEstateToTra(int traId, string estateId, string estateName)
        {
             _traEstatesRepository.AddEstateToTra(traId, estateId, estateName);
        }


        public bool AreUnusedEstates(List<string> traEsatateRefs)
        {
            return (traEsatateRefs.Intersect(_traEstatesRepository.GetAllUsedEstateRefs()).Any());
        }

    }
}
