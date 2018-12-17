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
        TRAInformation FindTRAInformation (int TRAId);
    }
}
