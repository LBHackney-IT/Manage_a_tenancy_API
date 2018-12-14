using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;
using ManageATenancyAPI.Repository;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class EstateAction : IEstateAction
    {
        private IEstateRepository _estateRepository;
        public EstateAction(IEstateRepository estateRepository)
        {
            _estateRepository = estateRepository;
        }

        public async Task<List<Estate>> GetEstates(IList<string> estateId)
        {
            var results = new List<Estate>();
            foreach (var uhProperty in await _estateRepository.GetEstates(estateId))
            {
                results.Add(Estate.FromModel(uhProperty));
            }
            return results;
        }

        public async Task<List<Estate>> GetEstatesNotInList(IList<string> usedEstates)
        {
            var results = new List<Estate>();
            foreach (var uhProperty in await _estateRepository.GetEstatesNotInList(usedEstates))
            {
                results.Add(Estate.FromModel(uhProperty));
            }
            return results;
        }
    }
}
