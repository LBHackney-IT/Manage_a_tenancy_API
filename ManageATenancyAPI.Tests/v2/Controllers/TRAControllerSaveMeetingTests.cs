using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.Tests.v2.Helper;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            _classUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
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
        public async Task returns_meeting_id()
        {
            var inputModel = SaveMeetingInputModelHelper.Create();

            _mockUseCase.Setup(s => s.ExecuteAsync(inputModel, It.IsAny<CancellationToken>())).ReturnsAsync(new SaveETRAMeetingOutputModel
            {
                MeetingId = Guid.NewGuid()
            });
            //act
            var response = await _classUnderTest.Post(inputModel);

            //assert
            var outputModel = response.GetResponseType<SaveETRAMeetingOutputModel>();
            outputModel.Should().NotBeNull();
            outputModel.MeetingId.Should().NotBeEmpty();
        }

        [Fact]
        public async Task returns_bad_request_when_invalid_model_is_passed()
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel();
            _classUnderTest.ModelState.AddModelError("TRAId", "Should not be null");
            //act
            var response = await _classUnderTest.Post(inputModel);

            //assert
            var badRequest = response as BadRequestObjectResult;
            badRequest.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            badRequest.Value.Should().NotBeNull();
        }
    }
}