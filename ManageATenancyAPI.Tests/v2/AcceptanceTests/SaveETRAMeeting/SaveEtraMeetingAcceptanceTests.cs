using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Xunit;
using FluentAssertions;
using ManageATenancyAPI.Actions.Housing.NHO;
using ManageATenancyAPI.Gateways.SaveEtraMeeting;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.Logging;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Tests.v2.Helper;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace ManageATenancyAPI.Tests.v2.AcceptanceTests.SaveETRAMeeting
{
    public class SaveEtraMeetingAcceptanceTests
    {
        private TRAController _classUnderTest;
        private ISaveEtraMeetingUseCase _useCase;
        private IJWTService _jwtService;

        public SaveEtraMeetingAcceptanceTests()
        {
            var serviceProvider = BuildServiceProvider();

            _jwtService = serviceProvider.GetService<IJWTService>();
            _useCase = serviceProvider.GetService<ISaveEtraMeetingUseCase>();
            _classUnderTest = new TRAController(_jwtService, null, _useCase);

            var headers = new KeyValuePair<string, StringValues>("Authorization",
                "Bearer eyJhbGciOiJIUzI1NiIsImtpZCI6IkhTMjU2IiwidHlwIjoiSldUIn0.eyJzdWIiOiJtaG9sZGVuIiwianRpIjoiIiwiQ3JlYXRlIG1lZXRpbmciOiJ7XCJlc3RhdGVPZmZpY2VyTG9naW5JZFwiOlwiMWYxYmI3MjctY2UxYi1lODExLTgxMTgtNzAxMDZmYWE2YTMxXCIsXCJvZmZpY2VySWRcIjpcIjFmMWJiNzI3LWNlMWItZTgxMS04MTE4LTcwMTA2ZmFhNmEzMVwiLFwidXNlcm5hbWVcIjpcIm1ob2xkZW5cIixcImZ1bGxOYW1lXCI6XCJNZWdhbiBIb2xkZW5cIixcImFyZWFNYW5hZ2VySWRcIjpcIjU1MTJjNDczLTk5NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwib2ZmaWNlclBhdGNoSWRcIjpcIjhlOTU4YTM3LTg2NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwiYXJlYUlkXCI6XCI2XCJ9IiwibmJmIjowLCJleHAiOjE1OTMwNzY2MjYsImlhdCI6MTU2MTQ1NDIyNiwiaXNzIjoiT3V0c3lzdGVtcyIsImF1ZCI6Ik1hbmFnZUFUZW5hbmN5In0.d7e_bDz1JnZdXjDASng67HWmC7s466lfQEDK-weyXCQ");

            _classUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _classUnderTest.Request.Headers.Add(headers);
        }

        private static ServiceProvider BuildServiceProvider()
        {
            IServiceCollection collection = new ServiceCollection();
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var startup = new Startup(config, new HostingEnvironment());
            startup.ConfigureServices(collection);
            var serviceProvider = collection.BuildServiceProvider();
            return serviceProvider;
        }

        [Fact]
        public async Task can_save_etra_meeting()
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {
                TRAId = 3,
                MeetingName = "New ETRA meeting",
                Issues = new List<MeetingIssue>
                {
                    new MeetingIssue
                    {
                        IssueTypeId = "100000501",
                        IssueLocationName = "De Beauvoir Estate  1-126 Fermain Court",
                        IssueNote = "Bad things have happened please fix"
                    },
                    new MeetingIssue
                    {
                        IssueTypeId = "100000501",
                        IssueLocationName = "De Beauvoir Estate  1-126 Fermain Court",
                        IssueNote = "Bad things have happened please fix 2"
                    }
                },
                SignOff = new SignOff
                {
                    Name = "Jeff Pinkham",
                    Role = "chair",
                    Signature = ""
                }
            };

            //act
            var response = await _classUnderTest.Post(inputModel).ConfigureAwait(false);
            //assert
            var outputModel = response.GetOKResponseType<SaveETRAMeetingOutputModel>();
            outputModel.Should().NotBeNull();
            outputModel.MeetingId.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(1, "100000501", "De Beauvoir Estate  1 - 126 Fermain Court", "Bad things have happened please fix")]
        [InlineData(2, "100000501", "De Beauvoir Estate  127 -256 Fermain Court", "Bad things have happened please fix")]
        public async Task can_save_etra_meeting_with_issues(int attendees, string issueTypeId, string issueLocationName, string note)
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {
                TRAId = 3,
                MeetingName = "New ETRA meeting",
                MeetingAttendance = new MeetingAttendees
                {
                    Attendees = 1
                },
                Issues = new List<MeetingIssue>
                {
                    new MeetingIssue
                    {
                        IssueTypeId = issueTypeId,
                        IssueLocationName = issueLocationName,
                        IssueNote = note
                    },
                    new MeetingIssue
                    {
                        IssueTypeId = issueTypeId,
                        IssueLocationName = $"{issueLocationName} 2",
                        IssueNote = $"{note} 2"
                    }
                },
                SignOff = new SignOff
                {
                    Name = "Jeff Pinkham",
                    Role = "chair",
                    Signature = ""
                }
            };

            //act
            var response = await _classUnderTest.Post(inputModel).ConfigureAwait(false);
            //assert
            var outputModel = response.GetOKResponseType<SaveETRAMeetingOutputModel>();
            outputModel.Should().NotBeNull();
            outputModel.Issues.Should().NotBeNullOrEmpty();
            outputModel.Issues.Count.Should().Be(inputModel.Issues.Count);

            for (var i = 0; i < outputModel.Issues.Count; i++)
            {
                var expectedIssue = inputModel.Issues[i];
                var issue = outputModel.Issues[i];

                issue.Id.Should().NotBeEmpty();
                issue.IssueLocationName.Should().Be(expectedIssue.IssueLocationName);
                issue.IssueTypeId.Should().Be(expectedIssue.IssueTypeId);
                issue.IssueNote.Should().Be(expectedIssue.IssueNote);
            }
        }
    }
}
