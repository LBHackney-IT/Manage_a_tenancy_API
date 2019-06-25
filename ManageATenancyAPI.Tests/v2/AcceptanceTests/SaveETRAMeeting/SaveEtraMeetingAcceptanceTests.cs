using System.Collections.Generic;
using System.Threading.Tasks;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Xunit;
using FluentAssertions;
using ManageATenancyAPI.Tests.v2.Helper;

namespace ManageATenancyAPI.Tests.v2.AcceptanceTests.SaveETRAMeeting
{
    public class SaveEtraMeetingAcceptanceTests
    {
        private TRAController _controller;
        private ISaveEtraMeetingUseCase _useCase;
        public SaveEtraMeetingAcceptanceTests()
        {
            _useCase = new SaveEtraMeetingUseCase();
            _controller = new TRAController(null, _useCase);
        }

        [Fact]
        public async Task can_save_etra_meeting()
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {
                MeetingAttendance = new MeetingAttendees
                {
                    Attendees = 1
                },
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
            var response = await _controller.Post(inputModel).ConfigureAwait(false);
            //assert
            var outputModel = response.GetResponseType<SaveETRAMeetingOutputModel>();
            outputModel.Should().NotBeNull();
            outputModel.MeetingId.Should().NotBeEmpty();
        }
    }
}
