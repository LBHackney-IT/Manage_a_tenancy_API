using System;
using ManageATenancyAPI.Database.DTO;

namespace ManageATenancyAPI.Repository.Interfaces
{
    public interface ITenancyRepository
    {
        NewTenancyLastRunDto GetLastRun();
        void UpdateLastRun(DateTime dateTime);
    }
}