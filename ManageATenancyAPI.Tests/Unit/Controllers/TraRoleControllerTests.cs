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
    public class TraRoleControllerTests: BaseTest
    {
        [Fact]
        public async Task ReturnListOfRoles()
        {
            var traRoleAction = new Mock<ITraRoleAction>();
            var traRoleController = new TraRoleController(traRoleAction.Object);
            IList<TraRole> returned = new List<TraRole>();
            traRoleAction.Setup(x =>x.GetRoles()).Returns(Task.FromResult(returned));
            var result = await traRoleController.GetRoles();
            Assert.Empty(result.Result);
        }


    }
}
