using System;
using System.Linq;
using ManageATenancyAPI.Database;
using ManageATenancyAPI.Database.DTO;
using ManageATenancyAPI.Database.Entities;
using ManageATenancyAPI.Repository.Interfaces;

namespace ManageATenancyAPI.Repository
{
    public class TenancyRepository : ITenancyRepository
    {
        private ITenancyContext _tenancyContext;
        
        public TenancyRepository(ITenancyContext tenancyContext)
        {
            _tenancyContext = tenancyContext;
        }
        
        public NewTenancyLastRunDto GetLastRun()
        {
            var lastRun = _tenancyContext.NewTenancyLastRun.OrderBy(x => x.LastRun).LastOrDefault();
            return lastRun == null ? null : new NewTenancyLastRunDto(lastRun);
        }

        public void UpdateLastRun(DateTime dateTime)
        {
            var newTenancyLastRunTime = new NewTenancyLastRun {LastRun = dateTime};
            _tenancyContext.NewTenancyLastRun.Add(newTenancyLastRunTime);
            
            _tenancyContext.SaveChanges();
        }
    }
}