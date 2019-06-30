using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Services.JWT.Models;
using ManageATenancyAPI.Tests.v2.Helper;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.Controllers
{
    public class TRAControllerSaveMeetingTests
    {
        private readonly TRAController _classUnderTest;
        private readonly Mock<ISaveEtraMeetingUseCase> _mockUseCase;
        private readonly IJWTService _jwtService;
        
        public TRAControllerSaveMeetingTests()
        {
            _mockUseCase = new Mock<ISaveEtraMeetingUseCase>();
            _jwtService = new JWTService();
            _classUnderTest = new TRAController(_jwtService,  _mockUseCase.Object,null);

            var headers = new KeyValuePair<string, StringValues>("Authorization",
                "Bearer eyJhbGciOiJIUzI1NiIsImtpZCI6IkhTMjU2IiwidHlwIjoiSldUIn0.eyJzdWIiOiJtaG9sZGVuIiwianRpIjoiIiwiQ3JlYXRlIG1lZXRpbmciOiJ7XCJlc3RhdGVPZmZpY2VyTG9naW5JZFwiOlwiMWYxYmI3MjctY2UxYi1lODExLTgxMTgtNzAxMDZmYWE2YTMxXCIsXCJvZmZpY2VySWRcIjpcIjFmMWJiNzI3LWNlMWItZTgxMS04MTE4LTcwMTA2ZmFhNmEzMVwiLFwidXNlcm5hbWVcIjpcIm1ob2xkZW5cIixcImZ1bGxOYW1lXCI6XCJNZWdhbiBIb2xkZW5cIixcImFyZWFNYW5hZ2VySWRcIjpcIjU1MTJjNDczLTk5NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwib2ZmaWNlclBhdGNoSWRcIjpcIjhlOTU4YTM3LTg2NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwiYXJlYUlkXCI6XCI2XCJ9IiwibmJmIjowLCJleHAiOjE1OTMwNzY2MjYsImlhdCI6MTU2MTQ1NDIyNiwiaXNzIjoiT3V0c3lzdGVtcyIsImF1ZCI6Ik1hbmFnZUFUZW5hbmN5In0.d7e_bDz1JnZdXjDASng67HWmC7s466lfQEDK-weyXCQ");

            _classUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _classUnderTest.Request.Headers.Add(headers);
        }

        [Fact]
        public async Task calls_use_case()
        {
            //arrange
            var inputModel = SaveMeetingInputModelHelper.Create();

            _mockUseCase.Setup(s => s.ExecuteAsync(inputModel, It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SaveEtraMeetingOutputModelOutputModel
            {
                Id = Guid.NewGuid()
            });
            //act
            var outputModel = await _classUnderTest.Post(inputModel);

            //assert
            _mockUseCase.Verify(s => s.ExecuteAsync(It.Is<SaveETRAMeetingInputModel>(m=> m == inputModel),It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task returns_meeting_id()
        {
            var inputModel = SaveMeetingInputModelHelper.Create();

            _mockUseCase.Setup(s => s.ExecuteAsync(inputModel, It.IsAny<IManageATenancyClaims>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SaveEtraMeetingOutputModelOutputModel
            {
                Id = Guid.NewGuid()
            });
            //act
            var response = await _classUnderTest.Post(inputModel);

            //assert
            var outputModel = response.GetResponseType<SaveEtraMeetingOutputModelOutputModel>();
            outputModel.Should().NotBeNull();
            outputModel.Id.Should().NotBeEmpty();
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