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
    public class BlockRepositoryTests: BaseTest
    {
        [Fact]
        public async Task GetBlocksByEstateId()
        {
            
            var estateId = "00078614";
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            var blockRepository = new BlockRepository(options);
            var result = await blockRepository.GetBlocksByEstateId(estateId);
            Assert.Equal(10, result.Count());
            Assert.True(result.ToList().All(x => x.EstateId == estateId));
        }
    }
}
