using System;
using Xunit;
using FluentAssertions;
using ManageATenancyAPI.Services.JWT;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class JWTServiceTests
    {
        private string _token;
        private string _secret;
        private IJWTService _classUnderTest;
        public JWTServiceTests()
        {
            _secret = Environment.GetEnvironmentVariable("HmacSecret");
            _classUnderTest = new JWTService();
        }

        [Fact]
        public void can_get_manage_a_tenancy_claims()
        {
            //arrange
            _token = "eyJhbGciOiJIUzI1NiIsImtpZCI6IkhTMjU2IiwidHlwIjoiSldUIn0.eyJzdWIiOiJtaG9sZGVuIiwianRpIjoiIiwiQ3JlYXRlIG1lZXRpbmciOiJ7XCJlc3RhdGVPZmZpY2VyTG9naW5JZFwiOlwiMWYxYmI3MjctY2UxYi1lODExLTgxMTgtNzAxMDZmYWE2YTMxXCIsXCJvZmZpY2VySWRcIjpcIjFmMWJiNzI3LWNlMWItZTgxMS04MTE4LTcwMTA2ZmFhNmEzMVwiLFwidXNlcm5hbWVcIjpcIm1ob2xkZW5cIixcImZ1bGxOYW1lXCI6XCJNZWdhbiBIb2xkZW5cIixcImFyZWFNYW5hZ2VySWRcIjpcIjU1MTJjNDczLTk5NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwib2ZmaWNlclBhdGNoSWRcIjpcIjhlOTU4YTM3LTg2NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwiYXJlYUlkXCI6XCI2XCJ9IiwibmJmIjowLCJleHAiOjE1OTMwNzY2MjYsImlhdCI6MTU2MTQ1NDIyNiwiaXNzIjoiT3V0c3lzdGVtcyIsImF1ZCI6Ik1hbmFnZUFUZW5hbmN5In0.d7e_bDz1JnZdXjDASng67HWmC7s466lfQEDK-weyXCQ";

            //act
            var claims = _classUnderTest.GetManageATenancyClaims(_token, _secret);
            //assert
            claims.Should().NotBeNull();
        }

        [Fact]
        public void can_create_meeting_claims()
        {
            //arrange 
            Guid id = Guid.NewGuid();
            //act
            var token = _classUnderTest.CreateManageATenancySingleMeetingToken(id, "", 0, _secret);
            //assert
            token.Should().NotBeNull();
        }

        [Fact]
        public void can_read_meeting_claims()
        {
            //arrange 
            Guid id = Guid.NewGuid();
            var token = _classUnderTest.CreateManageATenancySingleMeetingToken(id, "",0, _secret);
            //act
            var meetingClaims = _classUnderTest.GetMeetingIdClaims(token, _secret);
            //assert
            meetingClaims.MeetingId.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("Officer Name 1")]
        [InlineData("Officer Name 2")]
        public void can_read_officer_name(string officerName)
        {
            //arrange 
            Guid id = Guid.NewGuid();
            var token = _classUnderTest.CreateManageATenancySingleMeetingToken(id,officerName,0, _secret);
            //act
            var meetingClaims = _classUnderTest.GetMeetingIdClaims(token, _secret);
            //assert
            meetingClaims.OfficerName.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(60)]
        [InlineData(61)]
        public void can_read_tra_id(int traId)
        {
            //arrange 
            Guid id = Guid.NewGuid();
            var token = _classUnderTest.CreateManageATenancySingleMeetingToken(id,"",traId, _secret);
            //act
            var meetingClaims = _classUnderTest.GetMeetingIdClaims(token, _secret);
            //assert
            meetingClaims.TraId.Should().Be(traId);
        }
    }
}
