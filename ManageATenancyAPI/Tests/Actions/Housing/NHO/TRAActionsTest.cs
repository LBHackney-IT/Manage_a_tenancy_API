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
    }
}
