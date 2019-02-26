using System;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Controllers.Housing.NHO;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using MyPropertyAccountAPI.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace ManageATenancyAPI.Tests.Unit.Controllers
{
    public class ETRAControllerTest
    {
        private Mock<IETRAMeetingsAction> etraMeetingActions;
        private Mock<IOptions<AppConfiguration>> mockConfig;
        private Mock<IOptions<URLConfiguration>> urlMockConfig;
        private Mock<IHackneyGetCRM365Token> mockToken;
        public ETRAControllerTest()
        {
            etraMeetingActions = new Mock<IETRAMeetingsAction>();
            mockConfig = new Mock<IOptions<AppConfiguration>>();
            urlMockConfig = new Mock<IOptions<URLConfiguration>>();
            //set up mock token
            mockToken = new Mock<IHackneyGetCRM365Token>();
            mockToken.Setup(x => x.getCRM365AccessToken()).ReturnsAsync(It.IsAny<string>());
        }
        [Fact]
        public async Task successful_create_ETRA_Meeting_returns_an_object()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            var expected = new JObject();
            expected.Add("interactionid", "testid");
            expected.Add("ticketnumber", "testticket");
            etraMeetingActions.Setup(x => x.CreateETRAMeeting(It.IsAny<ETRAIssue>())).ReturnsAsync(HackneyResult<JObject>.Create(expected));

            var actual = etraController.Post(It.IsAny<ETRAIssue>()).Result;

            Assert.Equal(JsonConvert.SerializeObject(actual.Value), JsonConvert.SerializeObject(HackneyResult<JObject>.Create(expected)));
            Assert.Equal(actual.StatusCode, 201);
        }
        [Fact]
        public async Task create_ETRA_Meeting_returns_an_empty_object_when_result_is_empty()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            var expected = new JObject();
            expected.Add("interactionid", null);
            expected.Add("ticketnumber",null);
            etraMeetingActions.Setup(x => x.CreateETRAMeeting(It.IsAny<ETRAIssue>())).ReturnsAsync(HackneyResult<JObject>.Create(expected));

            var actual = etraController.Post(It.IsAny<ETRAIssue>()).Result;

            Assert.Equal(JsonConvert.SerializeObject(actual.Value), JsonConvert.SerializeObject(HackneyResult<JObject>.Create(expected)));
            Assert.Equal(actual.StatusCode, 201);
        }
        [Fact]
        public async Task create_ETRA_Meeting_returns_500_status_code_if_exception_has_been_thrown()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            etraMeetingActions.Setup(x => x.CreateETRAMeeting(It.IsAny<ETRAIssue>())).ThrowsAsync(new ServiceRequestException());

            var actual = etraController.Post(It.IsAny<ETRAIssue>()).Result;

            Assert.Equal(actual.StatusCode, 500);
        }
        [Fact]
        public async Task FinaliseMeeting_IncorrectMeetingId_ReturnsNotFound()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);
            const string id = "123id";
            etraMeetingActions.Setup(x => x.GetMeeting(id)).ReturnsAsync((ETRAMeeting)null);

            var actionResult = await etraController.FinaliseMeeting(id, It.IsAny<FinaliseETRAMeetingRequest>());

            Assert.IsType<NotFoundResult>(actionResult);
        }
        [Fact]
        public async Task FinaliseMeeting_IdOfAlreadyConfirmedMeeting_ReturnsForbidden()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);
            const string id = "123id";
            etraMeetingActions.Setup(x => x.GetMeeting(id)).ReturnsAsync(new ETRAMeeting { ConfirmationDate = DateTime.Now });

            var actionResult = await etraController.FinaliseMeeting(id, It.IsAny<FinaliseETRAMeetingRequest>());

            Assert.IsType<ForbidResult>(actionResult);
        }

        [Fact]
        public async Task FinaliseMeeting_ValidInput_ReturnsSuccessfulResponse()
        {
            const string id = "123id";
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);
            etraMeetingActions.Setup(x => x.GetMeeting(id)).ReturnsAsync(new ETRAMeeting { Id = id });
            etraMeetingActions.Setup(x => x.FinaliseMeeting(id, It.IsAny<FinaliseETRAMeetingRequest>())).ReturnsAsync(new FinaliseETRAMeetingResponse { Id = id, IsFinalised = true });

            var actionResult = await etraController.FinaliseMeeting(id, It.IsAny<FinaliseETRAMeetingRequest>());

            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);

            var value = okResult.Value as HackneyResult<FinaliseETRAMeetingResponse>;
            Assert.NotNull(value);

            Assert.Equal(id, value.Result.Id);
            Assert.True(value.Result.IsFinalised);
        }

        [Fact]
        public async Task FinaliseMeeting_WithNullMeetingId_ThrowsArgumentException()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);
            
            async Task act() => await etraController.FinaliseMeeting(null, null);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]
        public async Task FinaliseMeeting_WithEmptyStringMeetingId_ThrowsArgumentException()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            async Task act() => await etraController.FinaliseMeeting(string.Empty, null);

            await Assert.ThrowsAsync<ArgumentException>(act);
        }
    }
}
