using System;
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
            var traController = new TRAController(null, null, null, null, null, null, traRoleAssignmentAction.Object, null);
            traRoleAssignmentAction.Setup(x => x.AddRepresentative(3, "Test User", "client"));
            traController.AddRepresentative(3, "Test User", "client");
        }
        [Fact]
        public void RemoveRepresentative()
        {
            var traRoleAssignmentAction = new Mock<ITraRoleAssignmentAction>();
            var traController = new TRAController(null, null, null, null, null, null, traRoleAssignmentAction.Object, null);
            traRoleAssignmentAction.Setup(x => x.RemoveRepresentative(3, "Test User"));
            traController.RemoveRepresentative(3, "Test User");
        }
        [Fact]
        public async void ListRepresentative()
        {
            var traRoleAssignmentAction = new Mock<ITraRoleAssignmentAction>();
            var traController = new TRAController(null, null, null, null, null, null, traRoleAssignmentAction.Object, null);
            traRoleAssignmentAction.Setup(x => x.GetRepresentatives(3));
            traController.ListRepresentatives(3);
        }
        [Fact]
        public async void FindTRAInformation()
        {
            var traController = new TRAController(null, null, null, null, null, null, null, null);
            traController.GetTRAInformation(1);
        }

        [Fact]
        public async void FindTRA()
        {
            var traAction = new Mock<ITraAction>();
            var traEstateAction = new Mock<ITraEstateAction>();
            var estateAction = new Mock<IEstateAction>();
            var traController = new TRAController(null, null, null, null, traEstateAction.Object, estateAction.Object, null, traAction.Object);

            traAction.Setup(x => x.Find("Test001")).ReturnsAsync(new TRA(){AreaId = 1, Email = "test@test.com", Name = "Test001", PatchId = Guid.Empty, TRAId = 1,Notes = string.Empty});
            traAction.Setup(x => x.Exists("Test001")).ReturnsAsync(true);
            traEstateAction.Setup(x => x.AreUnusedEstates(It.IsAny<List<string>>())).Returns(true);
            estateAction.Setup(x => x.GetEstates(It.IsAny<List<string>>())).ReturnsAsync(new List<Estate>());


            await traController.UpdateTra(1,new TraRequest(){AreaId = 1,Email="test@test.com",EstateRefs=new List<string>(),Name="Test001",Notes =string.Empty,PatchId=Guid.Empty});
        }
    }
}
