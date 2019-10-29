﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;

namespace ManageATenancyAPI.Interfaces
{
    public interface ITRARepository
    {
        List<TRA> FindTRAsForPatch(string patchId);
        Task<bool> Exists(string traName);
        Task<bool> Exists(int traId);
        Task<TRA> Create(string name, string notes, string email, int areaId, Guid patchId);

        Task<TRA> Get(int traId);
        void UpdateNotes(int traId, string notes);
        void UpdateEmail(int traId, string email);
        TRAInformation FindTRAInformation (int TRAId);
        Task<TRA> Find(string traName);
    }
}
