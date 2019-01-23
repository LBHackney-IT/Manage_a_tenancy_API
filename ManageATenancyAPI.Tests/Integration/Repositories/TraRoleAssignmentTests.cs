using System;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Repository;
using Microsoft.Extensions.Options;
using ManageATenancyAPI.Interfaces;
using Xunit;
using Moq;

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

            ILoggerAdapter<TRARepository> logger = new Mock<ILoggerAdapter<TRARepository>>().Object;
            IDBAccessRepository genericRepository = new Mock<IDBAccessRepository>().Object;
            IOptions<AppConfiguration> config = new Mock<IOptions<AppConfiguration>>().Object;
            var traRepository = new TRARepository(logger, genericRepository, options, config);


            var tra = traRepository.Find("AddRemoveRole TRA");

            var roleList = await traRoleRepository.List();

            var traRoles0 = await traRoleAssignmentRepository.GetRoleAssignmentForTra(tra.Id);
            Assert.True(traRoles0.Count == 0);

            await traRoleAssignmentRepository.AddRoleAssignment(tra.Id, roleList.First().Role, "Test User 3");
            var traRoles1 = await traRoleAssignmentRepository.GetRoleAssignmentForTra(tra.Id);
            Assert.True(traRoles1.Count ==1);

            await traRoleAssignmentRepository.RemoveRoleAssignment(tra.Id, roleList.First().Role);
            var traRoles2 = await traRoleAssignmentRepository.GetRoleAssignmentForTra(tra.Id);
            Assert.True(traRoles2.Count==0);
        }

    }
}
