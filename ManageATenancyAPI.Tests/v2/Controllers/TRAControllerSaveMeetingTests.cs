using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.Tests.v2.Helper;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.Controllers
{
    public class TRAControllerSaveMeetingTests
    {
        private readonly TRAController _classUnderTest;
        private readonly Mock<ISaveEtraMeetingUseCase> _mockUseCase;
        

        public TRAControllerSaveMeetingTests()
        {
            _mockUseCase = new Mock<ISaveEtraMeetingUseCase>();
            _classUnderTest = new TRAController(null, _mockUseCase.Object);
        }

        [Fact]
        public async Task calls_use_case()
        {
            //arrange
            var inputModel = SaveMeetingInputModelHelper.Create();

            _mockUseCase.Setup(s => s.ExecuteAsync(inputModel, It.IsAny<CancellationToken>())).ReturnsAsync(new SaveETRAMeetingOutputModel
            {
                MeetingId = Guid.NewGuid()
            });
            //act
            var outputModel = await _classUnderTest.Post(inputModel);

            //assert
            _mockUseCase.Verify(s => s.ExecuteAsync(It.Is<SaveETRAMeetingInputModel>(m=> m == inputModel),It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task can_get_meeting_id()
        {
            var inputModel = SaveMeetingInputModelHelper.Create();

            _mockUseCase.Setup(s => s.ExecuteAsync(inputModel, It.IsAny<CancellationToken>())).ReturnsAsync(new SaveETRAMeetingOutputModel
            {
                MeetingId = Guid.NewGuid()
            });
            //act
            var outputModel = await _classUnderTest.Post(inputModel);

            //assert
            outputModel.
            outputModel.Should().NotBeNull();
            outputModel.MeetingId.Should().NotBeEmpty();
        }
    }
}