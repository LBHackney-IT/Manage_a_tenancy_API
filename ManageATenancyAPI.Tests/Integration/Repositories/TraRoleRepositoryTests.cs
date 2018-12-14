using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using Microsoft.Extensions.Options;
using Xunit;

namespace ManageATenancyAPI.Tests.Integration.Repositories
{
    [Collection("Database collection")]
    public class TraRoleRepositoryTests : BaseTest
    {

        [Fact]
        public async Task TraRoleTests_ListRoles()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
           var traRoleRepository = new TraRoleRepository(options);

            var roleList = await traRoleRepository.List();

            Assert.Equal(roleList.Count(), 4);
        }
    }
}
