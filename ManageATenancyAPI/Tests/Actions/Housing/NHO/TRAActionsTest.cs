using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.Actions.Housing.NHO
{
    public class TRAActionsTest
    {
        #region GetTRAForPatch
        [Fact]
        public async Task action_should_throw_missing_result_exception_if_result_is_missing()
        {
            var fakeData = new Faker();
           
            var fakePatchId = fakeData.Random.String();
            var repositoryMock = new Mock<ITRARepository>();
            repositoryMock.Setup(x => x.FindTRAsForPatch(fakePatchId)).Returns(()=>null);

            var mockLogger = new Mock<ILoggerAdapter<TRAActions>>();

            var actions = new TRAActions(mockLogger.Object,repositoryMock.Object);
            await Assert.ThrowsAsync<MissingTRAsForPatchException>(async () => await actions.GetTRAForPatch(fakePatchId));
        }

        [Fact]
        public async Task action_should_return_empty_object_if_no_matches_are_found()
        {
            var fakeData = new Faker();
            List<TRA> result = new List<TRA>();

            var fakePatchId = fakeData.Random.String();
            var repositoryMock = new Mock<ITRARepository>();
            repositoryMock.Setup(x => x.FindTRAsForPatch(fakePatchId)).Returns(result);

            var mockLogger = new Mock<ILoggerAdapter<TRAActions>>();

            var actions = new TRAActions(mockLogger.Object, repositoryMock.Object);

            var expectedResult = new
            {
                results = result
            };
            var actualResult = await actions.GetTRAForPatch(fakePatchId);
         
            Assert.Equal(expectedResult.ToString(), actualResult.ToString());
        }

        [Fact]
        public async Task action_should_an_object_if_matches_are_found()
        {
            var fakeData = new Faker();
            List<TRA> result = new List<TRA>();
            result.Add(new TRA()
            {
                AreaId = fakeData.Random.Int(),
                Name = fakeData.Random.String(),
                TRAId = fakeData.Random.Int()
            });

            var fakePatchId = fakeData.Random.String();
            var repositoryMock = new Mock<ITRARepository>();
            repositoryMock.Setup(x => x.FindTRAsForPatch(fakePatchId)).Returns(result);

            var mockLogger = new Mock<ILoggerAdapter<TRAActions>>();
            var expectedResult = new
            {
                results = result
            };
            var actions = new TRAActions(mockLogger.Object, repositoryMock.Object);
            var actualResult = actions.GetTRAForPatch(fakePatchId).Result;
            Assert.Equal(expectedResult.ToString(),actualResult.ToString());
        }
        #endregion

        #region GetTRAInformation
        [Fact]
        public async Task get_tra_information_action_should_throw_missing_result_exception_if_result_is_missing()
        {
            var fakeData = new Faker();

            var fakeTRAId = fakeData.Random.Int();
            var repositoryMock = new Mock<ITRARepository>();
            repositoryMock.Setup(x => x.FindTRAInformation(fakeTRAId)).Returns(() => null);

            var mockLogger = new Mock<ILoggerAdapter<TRAActions>>();

            var actions = new TRAActions(mockLogger.Object, repositoryMock.Object);
            await Assert.ThrowsAsync<MissingTRAsForPatchException>(async () => await actions.GetTRAInformation(fakeTRAId));
        }

        [Fact]
        public async Task get_tra_information_action_should_return_empty_object_if_no_matches_are_found()
        {
            var fakeData = new Faker();
            TRAInformation result = new TRAInformation();

            var fakeTRAId = fakeData.Random.Int();
            var repositoryMock = new Mock<ITRARepository>();
            repositoryMock.Setup(x => x.FindTRAInformation(fakeTRAId)).Returns(result);

            var mockLogger = new Mock<ILoggerAdapter<TRAActions>>();

            var actions = new TRAActions(mockLogger.Object, repositoryMock.Object);

            var expectedResult = new
            {
                results = result
            };
            var actualResult = await actions.GetTRAInformation(fakeTRAId);

            Assert.Equal(expectedResult.ToString(), actualResult.ToString());
        }

        [Fact]
        public async Task get_tra_information_action_should_an_object_if_matches_are_found()
        {
           var fakeData = new Faker();
           TRAInformation result = new TRAInformation();
           //add tra information
            result.PatchId = fakeData.Random.String();
            result.AreaId = fakeData.Random.Int();
            result.Name = fakeData.Random.String();
            result.TRAId = fakeData.Random.Int();
            result.Email = fakeData.Random.String();
           
            //add list of estates
            List<TRAEstate> estates= new List<TRAEstate>();
            estates.Add(new TRAEstate()
            {
                EstateName = fakeData.Random.String(),
                TRAId = fakeData.Random.Int(),
                EstateUHReference = fakeData.Random.String()
            });
            estates.Add(new TRAEstate()
            {
                EstateName = fakeData.Random.String(),
                TRAId = fakeData.Random.Int(),
                EstateUHReference = fakeData.Random.String()
            });

            //add list of roles assignment
            List<TRARolesAssignment> roles =new List<TRARolesAssignment>();
            roles.Add(new TRARolesAssignment()
            {
                TRAId = fakeData.Random.Int(),
                Role= fakeData.Random.String(),
                RoleName = fakeData.Random.String(),
                PersonName = fakeData.Random.String()
             
            });

            result.ListOfEstates = estates;
            result.ListOfRoles = roles;
            var fakeTRaId = fakeData.Random.Int();
            var repositoryMock = new Mock<ITRARepository>();
            repositoryMock.Setup(x => x.FindTRAInformation(fakeTRaId)).Returns(result);

            var mockLogger = new Mock<ILoggerAdapter<TRAActions>>();
            var expectedResult = new
            {
                results = result
            };
            var actions = new TRAActions(mockLogger.Object, repositoryMock.Object);
            var actualResult = actions.GetTRAInformation(fakeTRaId).Result;
            Assert.Equal(expectedResult.ToString(), actualResult.ToString());
        }
        #endregion
    }
}
