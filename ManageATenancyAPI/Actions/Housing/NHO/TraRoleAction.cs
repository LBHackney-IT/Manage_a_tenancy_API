using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Models.UniversalHousing;
using ManageATenancyAPI.Repository;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class TraRoleAction : ITraRoleAction
    {
        private ITraRoleRepository _traRoleRepository;
        public TraRoleAction(ITraRoleRepository traRoleRepository)
        {
            _traRoleRepository = traRoleRepository;
        }

        public async Task<IList<TraRole>> GetRoles()
        {
            return await _traRoleRepository.List(); 
        }
    }
}
