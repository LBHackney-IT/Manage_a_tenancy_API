using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Services.Housing;
using ManageATenancyAPI.Tests;

namespace ManageATenancyAPI.Factories.Housing
{
    public class HackneyUHWWarehouseServiceFactory
    {
        public IHackneyUHWWarehouseService build(IUHWWarehouseRepository uhwWarehouseRepository, ILoggerAdapter<AccountActions> logger)
        {
            if (TestStatus.IsRunningInTests == false)
            {
               return new HackneyUHWWarehouseService(uhwWarehouseRepository, logger);
            }
            else
            {
                return new FakeUHWWarehouseService();
            }
        }
    }
}
