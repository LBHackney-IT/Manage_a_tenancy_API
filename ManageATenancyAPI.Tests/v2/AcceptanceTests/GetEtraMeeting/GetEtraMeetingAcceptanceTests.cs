using System.Collections.Generic;
using System.Threading.Tasks;
using ManageATenancyAPI.Controllers.v2;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Xunit;
using FluentAssertions;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Tests.v2.Helper;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace ManageATenancyAPI.Tests.v2.AcceptanceTests.GetEtraMeeting
{
    public class GetEtraMeetingAcceptanceTests : AcceptanceTests
    {
        private TRAController _classUnderTest;
        private IGetEtraMeetingUseCase _useCase;
        private ISaveEtraMeetingUseCase _saveEtraMeetingUseCase;
        private IJWTService _jwtService;

        public GetEtraMeetingAcceptanceTests()
        {
            var serviceProvider = BuildServiceProvider();

            _jwtService = serviceProvider.GetService<IJWTService>();
            _useCase = serviceProvider.GetService<IGetEtraMeetingUseCase>();
            _saveEtraMeetingUseCase = serviceProvider.GetService<ISaveEtraMeetingUseCase>();
            _classUnderTest = new TRAController(_jwtService, _saveEtraMeetingUseCase, _useCase);

            var headers = new KeyValuePair<string, StringValues>("Authorization",
                "Bearer eyJhbGciOiJIUzI1NiIsImtpZCI6IkhTMjU2IiwidHlwIjoiSldUIn0.eyJzdWIiOiJtaG9sZGVuIiwianRpIjoiIiwiQ3JlYXRlIG1lZXRpbmciOiJ7XCJlc3RhdGVPZmZpY2VyTG9naW5JZFwiOlwiMWYxYmI3MjctY2UxYi1lODExLTgxMTgtNzAxMDZmYWE2YTMxXCIsXCJvZmZpY2VySWRcIjpcIjFmMWJiNzI3LWNlMWItZTgxMS04MTE4LTcwMTA2ZmFhNmEzMVwiLFwidXNlcm5hbWVcIjpcIm1ob2xkZW5cIixcImZ1bGxOYW1lXCI6XCJNZWdhbiBIb2xkZW5cIixcImFyZWFNYW5hZ2VySWRcIjpcIjU1MTJjNDczLTk5NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwib2ZmaWNlclBhdGNoSWRcIjpcIjhlOTU4YTM3LTg2NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwiYXJlYUlkXCI6XCI2XCJ9IiwibmJmIjowLCJleHAiOjE1OTMwNzY2MjYsImlhdCI6MTU2MTQ1NDIyNiwiaXNzIjoiT3V0c3lzdGVtcyIsImF1ZCI6Ik1hbmFnZUFUZW5hbmN5In0.d7e_bDz1JnZdXjDASng67HWmC7s466lfQEDK-weyXCQ");

            _classUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _classUnderTest.Request.Headers.Add(headers);
        }

        [Fact]
        public async Task can_get_saved_etra_meeting()
        {
            //arrange
            var inputModel = new SaveETRAMeetingInputModel
            {
                TRAId = 3,
                MeetingName = "New ETRA meeting",
                MeetingAttendance = new MeetingAttendees
                {
                    Attendees = 1
                },
            };

            //act
            var response = await _classUnderTest.Post(inputModel).ConfigureAwait(false);
            //assert
            var outputModel = response.GetOKResponseType<SaveEtraMeetingOutputModelOutputModel>();
            outputModel.Should().NotBeNull();
            outputModel.Id.Should().NotBeEmpty();
        }
    }
}
