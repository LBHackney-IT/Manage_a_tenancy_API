using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Xunit;
using FluentAssertions;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Tests.v2.Helper;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ManageATenancyAPI.Tests.v2.AcceptanceTests.GetEtraMeeting
{
    public class GetEtraMeetingAcceptanceTests : AcceptanceTests
    {
        private TRAController _classUnderTest;
        private IGetEtraMeetingUseCase _useCase;
        private ISaveEtraMeetingUseCase _saveEtraMeetingUseCase;
        private IJWTService _jwtService;

        public GetEtraMeetingAcceptanceTests()
        {
            var serviceProvider = BuildServiceProvider();

            _jwtService = serviceProvider.GetService<IJWTService>();
            _useCase = serviceProvider.GetService<IGetEtraMeetingUseCase>();
            _saveEtraMeetingUseCase = serviceProvider.GetService<ISaveEtraMeetingUseCase>();
            _classUnderTest = new TRAController(_jwtService, _saveEtraMeetingUseCase, _useCase, null);

            _classUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _classUnderTest.SetSaveMeetingTokenHeader();

        }

        [Theory]
        [InlineData(1, "100000501", "De Beauvoir Estate  1 - 126 Fermain Court", "Bad things have happened please fix")]
        [InlineData(2, "100000501", "De Beauvoir Estate  127 -256 Fermain Court", "Bad things have happened please fix")]
        public async Task can_save_then_get_etra_meeting(int attendees, string issueTypeId, string issueLocationName, string note)
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
            };

            var saveMeetingResponse = await _classUnderTest.Post(inputModel).ConfigureAwait(false);
            var outputModel = saveMeetingResponse.GetOKResponseType<SaveEtraMeetingOutputModel>();
            //set meeting Id Token
            var jwtToken = _jwtService.CreateManageATenancySingleMeetingToken(outputModel.Id, Environment.GetEnvironmentVariable("HmacSecret"));
            _classUnderTest.SetTokenHeader(jwtToken);

            //act
            var getMeetingResponse = await _classUnderTest.Get().ConfigureAwait(false);
            var getMeetingResponseOutputModel = getMeetingResponse.GetOKResponseType<GetEtraMeetingOutputModel>();
            //assert
            getMeetingResponseOutputModel.Should().BeEquivalentTo(outputModel);
            getMeetingResponseOutputModel.Should().BeEquivalentTo(outputModel);
        }
    }
}
