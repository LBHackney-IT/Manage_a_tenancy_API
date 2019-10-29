using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Gateways.SaveMeeting.SaveEtraMeetingSignOffMeeting;
using ManageATenancyAPI.Interfaces.Housing;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.UseCases.GetMeeting
{
    public class GetMeetingUseCaseTests
    {
        private IGetEtraMeetingUseCase _classUnderTest;
        private Mock<IETRAMeetingsAction> _mockMeetingActions;
        private Mock<IJpegPersistenceService> _mockJpegPersistenceService;
        public GetMeetingUseCaseTests()
        {
            _mockMeetingActions = new Mock<IETRAMeetingsAction>();
            _mockJpegPersistenceService = new Mock<IJpegPersistenceService>();
            _classUnderTest = new GetEtraMeetingUseCase(_mockMeetingActions.Object, _mockJpegPersistenceService.Object);
        }

        [Fact]
        public async Task calls_get_etra_meeting_actions()
        {
            //arrange
            var meetingId = Guid.NewGuid();
            var inputModel = new GetEtraMeetingInputModel
            {
                MeetingId = meetingId,
            };
            _mockMeetingActions.Setup(s => s.GetMeetingV2Async(It.Is<Guid>(m => m == meetingId), It.IsAny<CancellationToken>())).ReturnsAsync(new GetEtraMeetingOutputModel { });
            //act
            await _classUnderTest.ExecuteAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockMeetingActions.Verify(s => s.GetMeetingV2Async(It.Is<Guid>(m => m == meetingId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task calls_returns_get_meeting_output_model()
        {
            //arrange
            var meetingId = Guid.NewGuid();
            var inputModel = new GetEtraMeetingInputModel
            {
                MeetingId = meetingId,
            };
            _mockMeetingActions.Setup(s => s.GetMeetingV2Async(It.Is<Guid>(m => m == meetingId), It.IsAny<CancellationToken>())).ReturnsAsync(new GetEtraMeetingOutputModel { });

            //act
            var outputModel = await _classUnderTest.ExecuteAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            outputModel.Should().NotBeNull();
        }

        [Fact]
        public async Task calls_get_etra_meeting_issues_actions()
        {
            //arrange
            var meetingId = Guid.NewGuid();
            var inputModel = new GetEtraMeetingInputModel
            {
                MeetingId = meetingId,
            };
            _mockMeetingActions.Setup(s => s.GetMeetingV2Async(It.Is<Guid>(m => m == meetingId), It.IsAny<CancellationToken>())).ReturnsAsync(new GetEtraMeetingOutputModel { });
            //act
            await _classUnderTest.ExecuteAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockMeetingActions.Verify(s => s.GetETRAIssuesForMeeting(It.Is<Guid>(m => m == meetingId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task returns_get_etra_meeting_issues_actions()
        {
            //arrange
            var meetingId = Guid.NewGuid();
            var inputModel = new GetEtraMeetingInputModel
            {
                MeetingId = meetingId,
            };

            _mockMeetingActions.Setup(s => s.GetMeetingV2Async(It.Is<Guid>(m => m == meetingId), It.IsAny<CancellationToken>())).ReturnsAsync(new GetEtraMeetingOutputModel { });

            _mockMeetingActions.Setup(s => s.GetETRAIssuesForMeeting(It.Is<Guid>(m => m == meetingId), It.IsAny<CancellationToken>())).ReturnsAsync(new List<MeetingIssueOutputModel>
            {
                new MeetingIssueOutputModel{}
            });
            //act
            var outputModel = await _classUnderTest.ExecuteAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            outputModel.Issues.Should().NotBeEmpty();
        }

        [Fact]
        public async Task calls_jpeg_persistence_service()
        {
            //arrange
            var meetingId = Guid.NewGuid();
            var inputModel = new GetEtraMeetingInputModel
            {
                MeetingId = meetingId,
            };

            var signatureId = Guid.NewGuid();
            _mockMeetingActions.Setup(s => s.GetMeetingV2Async(It.Is<Guid>(m => m == meetingId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEtraMeetingOutputModel
                {
                    SignOff = new SignOff
                    {
                        SignatureId = signatureId
                    }
                });

            _mockMeetingActions.Setup(s => s.GetETRAIssuesForMeeting(It.Is<Guid>(m => m == meetingId), It.IsAny<CancellationToken>())).ReturnsAsync(new List<MeetingIssueOutputModel>
            {
                new MeetingIssueOutputModel
                { }
            });
            //act
            await _classUnderTest.ExecuteAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockJpegPersistenceService.Verify(s=> s.GetAsync(It.Is<string>(m=> m== signatureId.ToString())));
        }
    }
}
