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
    public class TraRoleAssignmentRepositoryTests : BaseTest
    {

        [Fact]
        public async Task TraRoleAssignmentTests_AddRoleAssignment()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            var traRoleAssignmentRepository = new TraRoleAssignmentRepository(options);
            var traRoleRepository = new TraRoleRepository(options);

            var roleList = await traRoleRepository.List();
            await traRoleAssignmentRepository.AddRoleAssignment(3, roleList.First().Role, "Test User 1");
            var traRoles = await traRoleAssignmentRepository.GetRoleAssignmentForTra(3);

            Assert.Equal(traRoles.First().TraId, 3);
            Assert.Equal(roleList.First().Role, roleList.First().Role);
            Assert.Equal(traRoles.First().PersonName, "Test User 1");
        }
        [Fact]
        public async Task TraRoleAssignmentTests_AddAndRemoveRoleAssignment()
        {
            var options = new OptionsWrapper<ConnStringConfiguration>(GetConfiguration<ConnStringConfiguration>(Config, "ConnectionStrings"));
            var traRoleAssignmentRepository = new TraRoleAssignmentRepository(options);
            var traRoleRepository = new TraRoleRepository(options);

            var roleList = await traRoleRepository.List();
            await traRoleAssignmentRepository.AddRoleAssignment(2, roleList.First().Role, "Test User 3");
            var traRoles = await traRoleAssignmentRepository.GetRoleAssignmentForTra(3);

            await traRoleAssignmentRepository.RemoveRoleAssignment(3, "Test User 3");
            await traRoleAssignmentRepository.GetRoleAssignmentForTra(3);
            Assert.True(traRoles.Count==0);


        }

    }
}
