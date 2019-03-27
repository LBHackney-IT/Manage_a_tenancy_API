﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Configuration;
using ManageATenancyAPI.Controllers.Housing.NHO;
using ManageATenancyAPI.Helpers;
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
        public async Task RecordAttendance_IncorrectMeetingId_ReturnsNotFound()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);
            const string id = "123id";
            etraMeetingActions.Setup(x => x.GetMeeting(id)).ReturnsAsync((ETRAMeeting)null);

            var request = new RecordETRAMeetingAttendanceRequest
            {
                TotalAttendees = 5,
                Councillors = "Joseph P. Bloggs",
                OtherCouncilStaff = "Jonathan H. Smith"
            };
            var actionResult = await etraController.RecordAttendance(id, request);

            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task RecordAttendance_ValidInput_ReturnsSuccessfulResponse()
        {
            var meeting = GetMeeting(false);
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);
            etraMeetingActions.Setup(x => x.GetMeeting(meeting.Id)).ReturnsAsync(meeting);
            var response = new RecordETRAMeetingAttendanceResponse
            {
                Councillors = string.Empty,
                MeetingId = meeting.Id,
                OtherCouncilStaff = string.Empty,
                Recorded = true,
                TotalAttendees = 5
            };
            etraMeetingActions.Setup(x => x.RecordETRAMeetingAttendance(meeting.Id, It.IsAny<RecordETRAMeetingAttendanceRequest>())).ReturnsAsync(response);

            var request = new RecordETRAMeetingAttendanceRequest
            {
                TotalAttendees = 5,
                Councillors = "Joseph P. Bloggs",
                OtherCouncilStaff = "Jonathan H. Smith"
            };
            var actionResult = await etraController.RecordAttendance(meeting.Id, request);

            var okResult = actionResult.Result as OkObjectResult;
            Assert.NotNull(okResult);

            var value = okResult.Value as RecordETRAMeetingAttendanceResponse;
            Assert.NotNull(value);

            Assert.Equal(meeting.Id, value.MeetingId);
            Assert.True(value.Recorded);
        }

        [Fact]
        public async Task RecordAttendance_WithNullMeetingId_ReturnsBadRequest()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            var actionResult = await etraController.RecordAttendance(null, null);

            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task RecordAttendance_WithEmptyStringMeetingId_ReturnsBadRequest()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            var actionResult = await etraController.RecordAttendance(string.Empty, null);

            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task RecordAttendance_WithNullRecordAttendanceRequest_ReturnsBadRequest()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            var actionResult = await etraController.RecordAttendance("123id", null);

            Assert.IsType<BadRequestResult>(actionResult.Result);
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
            var meeting = GetMeeting();
            etraMeetingActions.Setup(x => x.GetMeeting(meeting.Id)).ReturnsAsync(meeting);

            var actionResult = await etraController.FinaliseMeeting(meeting.Id, It.IsAny<FinaliseETRAMeetingRequest>());

            Assert.IsType<ForbidResult>(actionResult);
        }

        [Fact]
        public async Task FinaliseMeeting_ValidInput_ReturnsSuccessfulResponse()
        {
            var meeting = GetMeeting(false);
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);
            etraMeetingActions.Setup(x => x.GetMeeting(meeting.Id)).ReturnsAsync(meeting);
            etraMeetingActions.Setup(x => x.FinaliseMeeting(meeting.Id, It.IsAny<FinaliseETRAMeetingRequest>())).ReturnsAsync(new FinaliseETRAMeetingResponse { Id = meeting.Id, IsFinalised = true });

            var actionResult = await etraController.FinaliseMeeting(meeting.Id, It.IsAny<FinaliseETRAMeetingRequest>());

            var okResult = actionResult as OkObjectResult;
            Assert.NotNull(okResult);

            var value = okResult.Value as HackneyResult<FinaliseETRAMeetingResponse>;
            Assert.NotNull(value);

            Assert.Equal(meeting.Id, value.Result.Id);
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

        [Fact]
        public async Task AddETRAIssueResponse_NullRequest_ReturnBadRequest()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            var actionResult = await etraController.AddETRAIssueResponse(null);

            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task AddETRAIssueResponse_NotYetCompletedStatusAndNoProjectedCompletionDate_ReturnBadRequest()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            var request = new ETRAIssueResponseRequest
            {
                IsPublic = false,
                IssueId = "",
                IssueStatus = IssueStatus.NotYetCompleted,
                ProjectedCompletionDate = null,
                ResponseFrom = "",
                ResponseText = "",
                ServiceArea = ""
            };

            var actionResult = await etraController.AddETRAIssueResponse(request);

            Assert.IsType<BadRequestResult>(actionResult.Result);
        }

        [Fact]
        public async Task AddETRAIssueResponse_ValidRequest_ReturnsETRAIssueResponseModel()
        {
            var etraController = new ETRAController(etraMeetingActions.Object, null, null, urlMockConfig.Object, mockConfig.Object, mockToken.Object);

            var request = new ETRAIssueResponseRequest
            {
                IsPublic = false,
                IssueId = "",
                IssueStatus = IssueStatus.Completed,
                ProjectedCompletionDate = null,
                ResponseFrom = "",
                ResponseText = "",
                ServiceArea = ""
            };

            etraMeetingActions.Setup(x => x.GetIssue(It.IsAny<string>()))
                .ReturnsAsync(new ETRAIssue());
            etraMeetingActions.Setup(x => x.AddETRAIssueResponse(It.IsAny<ETRAIssueResponseRequest>(), It.IsAny<ETRAIssue>()))
                .ReturnsAsync(new ETRAIssueResponseModel());

            var actionResult = await etraController.AddETRAIssueResponse(request);

            Assert.IsType<OkObjectResult>(actionResult.Result);
            var objectResult = actionResult.Result as OkObjectResult;

            Assert.IsType<HackneyResult<ETRAIssueResponseModel>>(objectResult.Value);
        }

        public ETRAMeeting GetMeeting(bool isCompleted = true)
        {
            var meeting = new Dictionary<string,object> {
                { "hackney_tenancymanagementinteractionsid", "686c2e06-e639-e911-a973-00224807251a" },
                { "hackney_name", "CAS-07377-V5Q6Q1" },
                { "createdon", DateTime.Now.AddDays(-2) },
                { "modifiedon", DateTime.Now.AddDays(-1) },
                { "hackney_confirmationdate", null },
                { "hackney_signaturereference", null },
                { "hackney_signatoryrole", null },
                { "hackney_pdfreference", null },
                { "hackney_councillorsattendingmeeting", string.Empty },
                { "hackney_othercouncilstaffattendingmeeting", string.Empty },
                { "hackney_totalmeetingattendees", null }
            };

            if (isCompleted)
            {
                meeting["hackney_confirmationdate"] = DateTime.Now;
                meeting["hackney_signaturereference"] = Guid.Empty;
                meeting["hackney_signatoryrole"] = "Boss";
                meeting["hackney_pdfreference"] = Guid.Empty;
            }

            return ETRAMeeting.Create(meeting);
        }
    }
}
