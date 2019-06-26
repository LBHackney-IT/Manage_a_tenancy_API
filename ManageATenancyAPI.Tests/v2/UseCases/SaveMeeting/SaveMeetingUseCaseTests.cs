using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Gateways.SaveEtraMeeting;
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

        public SaveMeetingUseCaseTests()
        {
            _mockSaveMeetingGateway = new Mock<ISaveEtraMeetingGateway>();
            _classUnderTest = new SaveEtraMeetingUseCase(_mockSaveMeetingGateway.Object);
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
    }
}
