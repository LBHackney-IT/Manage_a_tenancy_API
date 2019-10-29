using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;

namespace ManageATenancyAPI.Repository
{
    public interface ITraRoleAssignmentRepository
    {
        Task AddRoleAssignment(int traId, string role, string personName);
        Task RemoveRoleAssignment(int traId, string role);
        Task<List<RoleAssignment>> GetRoleAssignmentForTra(int tra);
    }
}
