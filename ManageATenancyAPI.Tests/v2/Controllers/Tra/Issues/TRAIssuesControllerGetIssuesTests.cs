using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ManageATenancyAPI.Controllers.v2.Tra;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Tests.v2.Helper;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace ManageATenancyAPI.Tests.v2.Controllers.Tra.Issues
{
    public class TRAIssuesControllerGetIssuesTests
    {
        private readonly TRAIssuesController _classUnderTest;
        private readonly Mock<IEscalateIssuesUseCase> _mockUseCase;
        private readonly IJWTService _jwtService;
        private readonly Guid _meetingId;

        public TRAIssuesControllerGetIssuesTests()
        {
            _mockUseCase = new Mock<IEscalateIssuesUseCase>();
            _jwtService = new JWTService();
            _classUnderTest = new TRAIssuesController(_jwtService, _mockUseCase.Object);

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
        public async Task calls_use_case()
        {
            //arrange
            var id = Guid.NewGuid();
            _mockUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new EscalateIssuesOutputModel());
            //act
            var response = await _classUnderTest.Post();
            //assert
            _mockUseCase.Verify(v=> v.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task returns_output_model_from_use_case()
        {
            //arrange
            var id = Guid.NewGuid();
            _mockUseCase.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new EscalateIssuesOutputModel
            {
                SuccessfullyEscalatedIssues = new List<MeetingIssueOutputModel>
                {
                    new MeetingIssueOutputModel
                    {
                        Id = id
                    }
                }
            });
            //act
            var response = await _classUnderTest.Post();
            //assert
            response.GetOKResponseType<EscalateIssuesOutputModel>().SuccessfullyEscalatedIssues[0].Id.Should().Be(id);
        }
    }
}