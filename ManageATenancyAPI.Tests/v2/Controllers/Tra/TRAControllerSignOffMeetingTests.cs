using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.Tests.v2.Helper;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting;
using ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.Controllers.Tra
{
    public class TRAControllerSignOffMeetingTests
    {
        private readonly TRAController _classUnderTest;
        private readonly Mock<ISignOffMeetingUseCase> _mockUseCase;
        private readonly IJWTService _jwtService;
        private readonly Guid _meetingId;

        public TRAControllerSignOffMeetingTests()
        {
            _mockUseCase = new Mock<ISignOffMeetingUseCase>();
            _jwtService = new JWTService();
            _classUnderTest = new TRAController(_jwtService, null, null,_mockUseCase.Object);

            _meetingId = Guid.NewGuid();

            var token = _jwtService.CreateManageATenancySingleMeetingToken(_meetingId, "Jeff Pinkham", 60,
                Environment.GetEnvironmentVariable("HmacSecret"));

            var headers = new KeyValuePair<string, StringValues>("Authorization", $"Bearer {token}");

            _classUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _classUnderTest.Request.Headers.Add(headers);
        }

        [Fact]
        public async Task can_get_meeting_id_from_token()
        {
            //arrange
            _mockUseCase.Setup(s => s.ExecuteAsync(
                It.IsAny<SignOffMeetingInputModel>(),
                It.IsAny<IMeetingClaims>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new SignOffMeetingOutputModel
            {
                Id = _meetingId
            });
            var inputModel = new SignOffMeetingInputModel
            {
                
            };
            //act
            await _classUnderTest.Patch(inputModel).ConfigureAwait(false);
            //assert
            _mockUseCase.Verify(s => s.ExecuteAsync(It.IsAny<SignOffMeetingInputModel>(),
                It.Is<IMeetingClaims>(m => m.MeetingId == _meetingId),
                It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task returns_output_model_from_use_case()
        {
            //arrange
            _mockUseCase.Setup(s => s.ExecuteAsync(
                It.IsAny<SignOffMeetingInputModel>(),
                It.IsAny<IMeetingClaims>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(new SignOffMeetingOutputModel
            {
                Id = _meetingId
            });
            var inputModel = new SignOffMeetingInputModel();
            //act
            var response = await _classUnderTest.Patch(inputModel);
            //assert
            response.GetOKResponseType<SignOffMeetingOutputModel>().Id.Should().Be(_meetingId);
        }

        [Fact]
        public async Task returns_bad_request_when_invalid_model_is_passed()
        {
            //arrange
            var inputModel = new SignOffMeetingInputModel();
            _classUnderTest.ModelState.AddModelError("SignOff", "Should not be null");
            //act
            var response = await _classUnderTest.Patch(inputModel);

            //assert
            var badRequest = response as BadRequestObjectResult;
            badRequest.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequest.Value.Should().NotBeNull();
        }

    }
}