using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Tests;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Services.Housing;

namespace ManageATenancyAPI.Factories.Housing
{
    public class HackneyAccountsServiceFactory
    {
        public IHackneyHousingAPICall build()
        {
            if (TestStatus.IsRunningInTests == false)
            {
                return new HackneyHousingAPICall();
            }
            else
            {
                return new FakeHousingAPICall();
            }
        }
        
    }
}
