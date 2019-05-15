using System;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Repository.Interfaces;
using ManageATenancyAPI.Services.Interfaces;

namespace ManageATenancyAPI.Services
{
    public class NewTenancyService : INewTenancyService
    {
        private ITenancyRepository _tenancyRepository;
        private IClock _clock;
        
        public NewTenancyService(ITenancyRepository tenancyRepository, IClock clock)
        {
            _tenancyRepository = tenancyRepository;
            _clock = clock;
        }

        public void UpdateLastRetrieved(DateTime timestamp)
        {
            _tenancyRepository.UpdateLastRun(timestamp);
        }

        public DateTime GetLastRetrieved()
        {
            var lastRun = _tenancyRepository.GetLastRun();
            if (lastRun == null)
            {
                return _clock.Now.AddDays(-1);
            }
            return lastRun.LastRun;
        }
    }
}