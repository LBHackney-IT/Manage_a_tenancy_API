using System;

namespace ManageATenancyAPI.Services.Interfaces
{
    public interface INewTenancyService
    {
        void UpdateLastRetrieved(DateTime timestamp);
        DateTime GetLastRetrieved();
    }
}