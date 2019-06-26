using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.SaveEtraMeeting;
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

        [Fact]
        public async Task calls_create_meeting_gateway()
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {

            };
            //act
            await _classUnderTest.ExecuteAsync(inputModel, CancellationToken.None).ConfigureAwait(false);
            //assert
            _mockSaveMeetingGateway.Verify(s=> s.CreateEtraMeeting(It.IsAny<ETRAMeeting>(), It.IsAny<CancellationToken>()));
        }
    }
}
