using System;
using ManageATenancyAPI.Database;
using ManageATenancyAPI.Database.DTO;
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
            throw new System.NotImplementedException();
        }

        public void UpdateLastRun(DateTime dateTime)
        {
            throw new NotImplementedException();
        }
    }
}