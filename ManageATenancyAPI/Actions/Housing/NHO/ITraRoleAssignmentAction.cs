using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.Housing.NHO;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public interface ITraRoleAssignmentAction
    {
        void AddRepresentative(int traId, string personName, string role);
        void RemoveRepresentative(int traId, string personName);
       Task< List<RoleAssignment>> GetRepresentatives(int traId);
    }
}
