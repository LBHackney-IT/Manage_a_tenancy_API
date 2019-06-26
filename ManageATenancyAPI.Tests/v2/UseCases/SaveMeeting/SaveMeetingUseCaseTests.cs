using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingAttendance;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingIssue;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.UseCases.SaveMeeting
{
    public class SaveMeetingUseCaseTests
    {
        private ISaveEtraMeetingUseCase _classUnderTest;
        private Mock<ISaveEtraMeetingGateway> _mockSaveMeetingGateway;
        private Mock<ISaveEtraMeetingIssueGateway> _mockSaveMeetingIssueGateway;
        private Mock<ISaveEtraMeetingAttendanceGateway> _mockSaveMeetingAttendanceGateway;
        private Mock<ISaveEtraMeetingFinaliseMeetingGateway> _mockSaveMeetingFinaliseMeetingGateway;
        public SaveMeetingUseCaseTests()
        {
            _mockSaveMeetingGateway = new Mock<ISaveEtraMeetingGateway>();
            _mockSaveMeetingIssueGateway = new Mock<ISaveEtraMeetingIssueGateway>();
            _mockSaveMeetingAttendanceGateway = new Mock<ISaveEtraMeetingAttendanceGateway>();
            _mockSaveMeetingFinaliseMeetingGateway = new Mock<ISaveEtraMeetingFinaliseMeetingGateway>();
            _classUnderTest = new SaveEtraMeetingUseCase(_mockSaveMeetingGateway.Object, _mockSaveMeetingIssueGateway.Object, _mockSaveMeetingAttendanceGateway.Object, _mockSaveMeetingFinaliseMeetingGateway.Object);
        }

        [Theory]
        [InlineData(1, "test meeting")]
        [InlineData(2, "test meeting 2")]
        public async Task calls_create_meeting_gateway(int traId, string meetingName)
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {
                TRAId = traId,
                MeetingName = meetingName
            };
            //act
            await _classUnderTest.ExecuteAsync(inputModel, It.IsAny<IManageATenancyClaims>(), CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockSaveMeetingGateway.Verify(s=> s.CreateEtraMeeting(It.Is<ETRAMeeting>(m => m.MeetingName.Equals(meetingName) && m.TraId == traId),It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>()));
        }

        [Theory]
        [InlineData(1, "test meeting")]
        [InlineData(2, "test meeting 2")]
        public async Task returns_meeting_id_from_gateway(int traId, string meetingName)
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {
                TRAId = traId,
                MeetingName = meetingName
            };

            var newGuid = Guid.NewGuid();
            _mockSaveMeetingGateway.Setup(s => s.CreateEtraMeeting(It.IsAny<ETRAMeeting>(),
                It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>())).ReturnsAsync(newGuid);
            //act
            var response = await _classUnderTest.ExecuteAsync(inputModel, It.IsAny<IManageATenancyClaims>(), CancellationToken.None).ConfigureAwait(false);
            //assert
            response.MeetingId.Should().Be(newGuid);
        }

        [Theory]
        [InlineData( "100000501", "De Beauvoir Estate  1 - 126 Fermain Court", "Bad things have happened please fix")]
        [InlineData( "100000501", "De Beauvoir Estate  127 -256 Fermain Court", "Bad things have happened please fix")]
        public async Task calls_save_meeting_issue_gateway(string issueTypeId, string issueLocationName, string note)
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
            await _classUnderTest.ExecuteAsync(inputModel, It.IsAny<IManageATenancyClaims>(), CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockSaveMeetingIssueGateway.Verify(s => s.CreateEtraMeetingIssue(It.IsAny<ETRAMeeting>(),It.Is<MeetingIssue>(m => m == inputModel.Issues[0]), It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>()),Times.Once);
            _mockSaveMeetingIssueGateway.Verify(s => s.CreateEtraMeetingIssue(It.IsAny<ETRAMeeting>(), It.Is<MeetingIssue>(m => m == inputModel.Issues[1]), It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData( "100000501", "De Beauvoir Estate  1 - 126 Fermain Court", "Bad things have happened please fix")]
        [InlineData( "100000501", "De Beauvoir Estate  127 -256 Fermain Court", "Bad things have happened please fix")]
        public async Task returns_issues_with_id(string issueTypeId, string issueLocationName, string note)
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

            _mockSaveMeetingIssueGateway.Setup(s => s.CreateEtraMeetingIssue(It.IsAny<ETRAMeeting>(),
                    It.Is<MeetingIssue>(m=> m == inputModel.Issues[0]),
                    It.IsAny<IManageATenancyClaims>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MeetingIssueOutputModel
                {
                    Id = Guid.NewGuid(),
                    IssueNote = inputModel.Issues[0].IssueNote,
                    IssueTypeId = inputModel.Issues[0].IssueTypeId,
                    IssueLocationName = inputModel.Issues[0].IssueLocationName
                });

            _mockSaveMeetingIssueGateway.Setup(s => s.CreateEtraMeetingIssue(It.IsAny<ETRAMeeting>(),
                    It.Is<MeetingIssue>(m => m == inputModel.Issues[1]),
                    It.IsAny<IManageATenancyClaims>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MeetingIssueOutputModel
                {
                    Id = Guid.NewGuid(),
                    IssueNote = inputModel.Issues[1].IssueNote,
                    IssueTypeId = inputModel.Issues[1].IssueTypeId,
                    IssueLocationName = inputModel.Issues[1].IssueLocationName
                });

            //act
            var outputModel = await _classUnderTest.ExecuteAsync(inputModel, It.IsAny<IManageATenancyClaims>(), CancellationToken.None).ConfigureAwait(false);
            //assert
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

        [Theory]
        [InlineData(1, "Jeff JohnJeff", "Person b ")]
        [InlineData(2, "Jeff NotJohnJeff", "Person A")]
        public async Task calls_record_attendance_gateway(int attendees, string councillors, string hackneyStaff)
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {

                MeetingAttendance = new MeetingAttendees
                {
                    Attendees = attendees,
                    Councillors = councillors,
                    HackneyStaff = hackneyStaff
                },
            };
            //act
            await _classUnderTest.ExecuteAsync(inputModel, It.IsAny<IManageATenancyClaims>(), CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockSaveMeetingAttendanceGateway.Verify(s => s.CreateEtraAttendance(It.IsAny<ETRAMeeting>(), It.Is<MeetingAttendees>(m => m == inputModel.MeetingAttendance), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("chair", "Jeff JohnJeff")]
        [InlineData("secretary", "Jeff NotJohnJeff")]
        public async Task calls_finalise_meeting_gateway(string role, string name)
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {

                SignOff = new SignOff
                {
                    Role = role,
                    Signature = "",
                    Name = name
                }
            };
            //act
            await _classUnderTest.ExecuteAsync(inputModel, It.IsAny<IManageATenancyClaims>(), CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockSaveMeetingFinaliseMeetingGateway.Verify(s => s.FinaliseMeeting(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
