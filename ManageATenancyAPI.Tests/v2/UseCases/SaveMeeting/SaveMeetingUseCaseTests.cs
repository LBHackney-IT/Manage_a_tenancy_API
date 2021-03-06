﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Gateways.CloseMeeting;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeeting;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingAttendance;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingIssue;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Services.Email;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.Tests.Unit.Services;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary;
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
        private Mock<ISaveEtraMeetingSignOffMeetingGateway> _mockSaveMeetingFinaliseMeetingGateway;
        private Mock<ISendTraConfirmationEmailGateway> _mockEmailService;
        private Mock<ICloseETRAMeetingGateway> _mockCloseMeeting;
        public SaveMeetingUseCaseTests()
        {
            _mockSaveMeetingGateway = new Mock<ISaveEtraMeetingGateway>();
            _mockSaveMeetingIssueGateway = new Mock<ISaveEtraMeetingIssueGateway>();
            _mockSaveMeetingAttendanceGateway = new Mock<ISaveEtraMeetingAttendanceGateway>();
            _mockSaveMeetingFinaliseMeetingGateway = new Mock<ISaveEtraMeetingSignOffMeetingGateway>();
            _mockEmailService = new Mock<ISendTraConfirmationEmailGateway>();
            _mockCloseMeeting = new Mock<ICloseETRAMeetingGateway>();
            _classUnderTest = new SaveEtraMeetingUseCase(
                _mockSaveMeetingGateway.Object,
                _mockSaveMeetingIssueGateway.Object,
                _mockSaveMeetingAttendanceGateway.Object,
                _mockSaveMeetingFinaliseMeetingGateway.Object,
                _mockEmailService.Object,
                _mockCloseMeeting.Object);

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
            var outputModel = new ETRAMeetingOutPutModel
            {
                IncidentId = Guid.NewGuid(),
                InteractionId = Guid.NewGuid(),
            };

            _mockSaveMeetingGateway.Setup(s => s.CreateEtraMeeting(It.IsAny<ETRAMeeting>(),
                It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>())).ReturnsAsync(outputModel);
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

            var outputModel = new ETRAMeetingOutPutModel
            {
                IncidentId = Guid.NewGuid(),
                 InteractionId= Guid.NewGuid(),
            };
           
            _mockSaveMeetingGateway.Setup(s => s.CreateEtraMeeting(It.IsAny<ETRAMeeting>(),
                It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>())).ReturnsAsync(outputModel);
            //act
            var response = await _classUnderTest.ExecuteAsync(inputModel, new ManageATenancyClaims(), CancellationToken.None).ConfigureAwait(false);
            //assert
            response.Id.Should().Be(outputModel.InteractionId);
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
                        IssueType = new IssueType
                        { IssueId=issueTypeId
                        },
                        Location=new Location
                        {
                            Name=issueLocationName
                        },
                        Notes = note
                    },
                    new MeetingIssue
                    {
                         IssueType = new IssueType
                        { IssueId=issueTypeId
                        },
                        Location=new Location
                        {
                            Name= $"{issueLocationName} 2",
                        },
                        Notes = $"{note} 2"

                    }
                },
                SignOff = new SignOff
                {
                    Name = "Jeff Pinkham",
                    Role = "chair",
                    Signature = ""
                }
            };

            var outputModel = new ETRAMeetingOutPutModel
            {
                IncidentId = Guid.NewGuid(),
                InteractionId = Guid.NewGuid(),
            };

            _mockSaveMeetingGateway.Setup(s => s.CreateEtraMeeting(It.IsAny<ETRAMeeting>(),
                It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>())).ReturnsAsync(outputModel);

            _mockSaveMeetingFinaliseMeetingGateway.Setup(s => s.SignOffMeetingAsync(
                It.IsAny<Guid>(),
                It.Is<SignOff>(m => m == inputModel.SignOff), It.IsAny<CancellationToken>())).ReturnsAsync(
                new SignOffMeetingOutputModel { });
            //act
            await _classUnderTest.ExecuteAsync(inputModel, new ManageATenancyClaims(), CancellationToken.None).ConfigureAwait(false);
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
                        IssueType = new IssueType
                        { IssueId=issueTypeId
                        },
                        Location=new Location
                        {
                            Name=issueLocationName
                        },
                        Notes = note
                    },
                    new MeetingIssue
                    {
                         IssueType = new IssueType
                        { IssueId=issueTypeId
                        },
                        Location=new Location
                        {
                            Name= $"{issueLocationName} 2",
                        },
                        Notes = $"{note} 2"

                    }
                },
                SignOff = new SignOff
                {
                    Name = "Jeff Pinkham",
                    Role = "chair",
                    Signature = ""
                }
            };
            var meetingOutputModel = new ETRAMeetingOutPutModel
            {
                IncidentId = Guid.NewGuid(),
                InteractionId = Guid.NewGuid(),
            };

            _mockSaveMeetingGateway.Setup(s => s.CreateEtraMeeting(It.IsAny<ETRAMeeting>(),
                It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>())).ReturnsAsync(meetingOutputModel);

            _mockSaveMeetingFinaliseMeetingGateway.Setup(s => s.SignOffMeetingAsync(
                It.IsAny<Guid>(),
                It.Is<SignOff>(m => m == inputModel.SignOff), It.IsAny<CancellationToken>())).ReturnsAsync(
                new SignOffMeetingOutputModel { });

            _mockSaveMeetingIssueGateway.Setup(s => s.CreateEtraMeetingIssue(It.IsAny<ETRAMeeting>(),
                    It.Is<MeetingIssue>(m=> m == inputModel.Issues[0]),
                    It.IsAny<IManageATenancyClaims>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MeetingIssueOutputModel
                {
                    Id = Guid.NewGuid(),
                    Notes = inputModel.Issues[0].Notes,
                    IssueType = inputModel.Issues[0].IssueType,
                    Location = inputModel.Issues[0].Location
                });

            _mockSaveMeetingIssueGateway.Setup(s => s.CreateEtraMeetingIssue(It.IsAny<ETRAMeeting>(),
                    It.Is<MeetingIssue>(m => m == inputModel.Issues[1]),
                    It.IsAny<IManageATenancyClaims>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MeetingIssueOutputModel
                {
                    Id = Guid.NewGuid(),
                    Notes = inputModel.Issues[1].Notes,
                    IssueType = inputModel.Issues[1].IssueType,
                    Location = inputModel.Issues[1].Location
                });

            //act
            var outputModel = await _classUnderTest.ExecuteAsync(inputModel, new ManageATenancyClaims(), CancellationToken.None).ConfigureAwait(false);
            //assert
            outputModel.Should().NotBeNull();
            outputModel.Issues.Should().NotBeNullOrEmpty();
            outputModel.Issues.Count.Should().Be(inputModel.Issues.Count);

            for (var i = 0; i < outputModel.Issues.Count; i++)
            {
                var expectedIssue = inputModel.Issues[i];
                var issue = outputModel.Issues[i];

                issue.Id.Should().NotBeEmpty();
                issue.Location.Name.Should().Be(expectedIssue.Location.Name);
                issue.IssueType.Should().Be(expectedIssue.IssueType);
                issue.Notes.Should().Be(expectedIssue.Notes);
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
            var meetingOutputModel = new ETRAMeetingOutPutModel
            {
                IncidentId = Guid.NewGuid(),
                InteractionId = Guid.NewGuid(),
            };

            _mockSaveMeetingGateway.Setup(s => s.CreateEtraMeeting(It.IsAny<ETRAMeeting>(),
                It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>())).ReturnsAsync(meetingOutputModel);
            //act
            await _classUnderTest.ExecuteAsync(inputModel, new ManageATenancyClaims(), CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockSaveMeetingAttendanceGateway.Verify(s => s.CreateEtraAttendance(It.IsAny<ETRAMeeting>(), It.Is<MeetingAttendees>(m => m == inputModel.MeetingAttendance), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("chair", "Jeff JohnJeff")]
        [InlineData("secretary", "Jeff NotJohnJeff")]
        public async Task calls_finalise_meeting_gateway_if_signoff_is_present(string role, string name)
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

            _mockSaveMeetingFinaliseMeetingGateway.Setup(s => s.SignOffMeetingAsync(
                It.IsAny<Guid>(), 
                It.Is<SignOff>(m => m == inputModel.SignOff), It.IsAny<CancellationToken>())).ReturnsAsync(
                new SignOffMeetingOutputModel{ });
            //act
            await _classUnderTest.ExecuteAsync(inputModel, new ManageATenancyClaims(), CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockSaveMeetingFinaliseMeetingGateway.Verify(s => s.SignOffMeetingAsync(It.IsAny<Guid>(), It.Is<SignOff>(m=> m == inputModel.SignOff), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task does_not_call_finalise_meeting_gateway_if_signoff_is_not_present()
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {
                SignOff = null
            };
            //act
            await _classUnderTest.ExecuteAsync(inputModel, new ManageATenancyClaims(), CancellationToken.None)
                .ConfigureAwait(false);
            //assert
            _mockSaveMeetingFinaliseMeetingGateway.Verify(
                s => s.SignOffMeetingAsync(It.IsAny<Guid>(), It.Is<SignOff>(m => m == inputModel.SignOff),
                    It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("chair", "Jeff JohnJeff")]
        [InlineData("secretary", "Jeff NotJohnJeff")]
        public async Task calls_email_service(string role, string name)
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
            IManageATenancyClaims claims = new ManageATenancyClaims
            {
                FullName = name,
            };

            var meetingOutputModel = new ETRAMeetingOutPutModel
            {
                IncidentId = Guid.NewGuid(),
                InteractionId = Guid.NewGuid(),
            };

            _mockSaveMeetingGateway.Setup(s => s.CreateEtraMeeting(It.IsAny<ETRAMeeting>(),
                It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>())).ReturnsAsync(meetingOutputModel);
            //act
            await _classUnderTest.ExecuteAsync(inputModel, claims, CancellationToken.None)
                .ConfigureAwait(false);
            //assert
            _mockEmailService.Verify(s => s.SendTraConfirmationEmailAsync(
                It.Is<SendTraConfirmationEmailInputModel>(m=> 
                    m.OfficerName.Equals(claims.FullName)), 
                It.IsAny<CancellationToken>()),Times.Once);
        }
    }
}
