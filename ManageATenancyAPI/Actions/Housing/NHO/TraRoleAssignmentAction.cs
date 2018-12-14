using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public class TraRoleAssignmentAction : ITraRoleAssignmentAction
    {
        private ITraRoleAssignmentRepository _traRoleAssignmentRepository;
        public TraRoleAssignmentAction(ITraRoleAssignmentRepository traRoleAssignmentRepository)
        {
            _traRoleAssignmentRepository = traRoleAssignmentRepository;
        }
        public void AddRepresentative(int traId,string personName, string role)
        {
            _traRoleAssignmentRepository.AddRoleAssignment(traId, role, personName);
        }

        public void RemoveRepresentative(int traId, string personName)
        {
            _traRoleAssignmentRepository.RemoveRoleAssignment(traId,  personName);
        }

        public async Task<List<RoleAssignment>> GetRepresentatives(int traId)
        {
            return await GetRepresentatives(traId);
        }
    }
}
