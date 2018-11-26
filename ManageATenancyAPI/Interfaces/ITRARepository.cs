using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;

namespace ManageATenancyAPI.Interfaces
{
    public interface ITRARepository
    {
        List<TRA> FindTRAsForPatch(string patchId);
        bool Exists(string traName);
        TRA Create(string name, string email, int areaId, Guid patchId);
    }
}
