using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Controllers.Housing.NHO;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.Repository;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Controllers
{
    public class TraControllerTests : BaseTest
    {

        [Fact]
        public void AddRepresentative()
        {

            var traRoleAssignmentAction = new Mock<ITraRoleAssignmentAction>();
            var traController = new TRAController(null, null, null, null, null, null, traRoleAssignmentAction.Object);
            traRoleAssignmentAction.Setup(x => x.AddRepresentative(3, "Test User", "client"));
            traController.AddRepresentative(3, "Test User", "client");
        }

        [Fact]
        public void RemoveRepresentative()
        {
            var traRoleAssignmentAction = new Mock<ITraRoleAssignmentAction>();
            var traController = new TRAController(null, null, null, null, null, null, traRoleAssignmentAction.Object);
            traRoleAssignmentAction.Setup(x => x.RemoveRepresentative(3, "Test User"));
            traController.RemoveRepresentative(3, "Test User");
        }

        [Fact]
        public async void ListRepresentative()
        {
            var traRoleAssignmentAction = new Mock<ITraRoleAssignmentAction>();
            var traController = new TRAController(null, null, null, null, null, null, traRoleAssignmentAction.Object);
            traRoleAssignmentAction.Setup(x => x.GetRepresentatives(3));
            traController.ListRepresentatives(3);
        }



    }
}
