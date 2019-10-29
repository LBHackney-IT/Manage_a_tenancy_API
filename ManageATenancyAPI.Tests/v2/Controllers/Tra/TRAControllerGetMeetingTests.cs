using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.Controllers.v2.Tra;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Tests.v2.Helper;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.Controllers.Tra
{
    public class TRAControllerGetMeetingTests
    {
        private readonly TRAController _classUnderTest;
        private readonly Mock<IGetEtraMeetingUseCase> _mockUseCase;
        private readonly IJWTService _jwtService;
        private readonly Guid _meetingId;

        public TRAControllerGetMeetingTests()
        {
            _mockUseCase = new Mock<IGetEtraMeetingUseCase>();
            _jwtService = new JWTService();
            _classUnderTest = new TRAController(_jwtService, null, _mockUseCase.Object, null);

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
            _mockUseCase.Setup(s => s.ExecuteAsync(It.Is<GetEtraMeetingInputModel>(m => m.MeetingId == _meetingId),
                It.IsAny<CancellationToken>())).ReturnsAsync(new GetEtraMeetingOutputModel
            {
                Id = _meetingId
            });
            //act
            var response = await _classUnderTest.Get();
            //assert
            _mockUseCase.Verify(s => s.ExecuteAsync(It.Is<GetEtraMeetingInputModel>(m => m.MeetingId == _meetingId),
                It.IsAny<CancellationToken>()), Times.Once());  
        }

        [Fact]
        public async Task returns_output_model_from_use_case()
        {
            //arrange
            _mockUseCase.Setup(s => s.ExecuteAsync(It.Is<GetEtraMeetingInputModel>(m => m.MeetingId == _meetingId),
                It.IsAny<CancellationToken>())).ReturnsAsync(new GetEtraMeetingOutputModel
            {
                Id = _meetingId
            });
            //act
            var response = await _classUnderTest.Get();
            //assert
            response.GetOKResponseType<GetEtraMeetingOutputModel>().Id.Should().Be(_meetingId);
        }


        [Fact]
        public async Task returns_unauthorised_when_invalid_token_is_passed()
        {
            //arrange
            
            _classUnderTest.Request.Headers.Clear();
            //act
            var response = await _classUnderTest.Get();

            //assert
            var badRequest = response as UnauthorizedResult;
            badRequest.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }
    }
}
