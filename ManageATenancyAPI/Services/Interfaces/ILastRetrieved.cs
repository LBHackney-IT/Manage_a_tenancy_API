using System;

namespace ManageATenancyAPI.Services.Interfaces
{
    public interface ILastRetrieved
    {
        void UpdateLastRun(DateTime timestamp);
        DateTime GetLastRun();
    }
}