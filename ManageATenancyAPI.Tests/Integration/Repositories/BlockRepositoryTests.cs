using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Repository;
using Microsoft.Extensions.Options;
using Xunit;

namespace ManageATenancyAPI.Tests.Integration.Repositories
{
    public class BlockRepositoryTests
    {


        [Fact]
        public async Task GetBlocksByEstateId()
        {
            var estateId = "00078614";
            var options = new OptionsWrapper<ConnStringConfiguration>(new ConnStringConfiguration() { UHWReportingWarehouse = "Server=10.80.65.49;Database=StagedDB;User Id=reports; Password=reports" });
            var blockRepository = new BlockRepository(options);
            var result = await blockRepository.GetBlocksByEstateId(estateId);
            Assert.Equal(10, result.Count());
            Assert.True(result.ToList().All(x => x.EstateId == estateId));
        }
    }
}
