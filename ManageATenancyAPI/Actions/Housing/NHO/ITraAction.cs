using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;

namespace ManageATenancyAPI.Actions.Housing.NHO
{
    public interface ITraAction
    {
        Task<bool> Exists(string traName);
        Task<TRA> Create(string name, string notes, string email, int areaId, Guid patchId);

        void UpdateNotes(int traId, string notes);
        void UpdateEmail(int traId, string email);
        Task<TRA> Find(string traName);
    }
}
