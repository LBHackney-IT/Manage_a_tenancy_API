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
            _token = "eyJhbGciOiJIUzI1NiIsImtpZCI6IkhTMjU2IiwidHlwIjoiSldUIn0.eyJzdWIiOiJtaG9sZGVuIiwianRpIjoiIiwibWVldGluZyI6IntcImVzdGF0ZU9mZmljZXJMb2dpbklkXCI6XCIyMDFiYjcyNy1jZTFiLWU4MTEtODExOC03MDEwNmZhYTZhMzFcIixcIm9mZmljZXJJZFwiOlwiMWYxYmI3MjctY2UxYi1lODExLTgxMTgtNzAxMDZmYWE2YTMxXCIsXCJmdWxsTmFtZVwiOlwiTWVnYW4gSG9sZGVuXCIsXCJhcmVhTWFuYWdlcklkXCI6XCI1NTEyYzQ3My05OTUzLWU4MTEtODEyNi03MDEwNmZhYWY4YzFcIixcIm9mZmljZXJQYXRjaElkXCI6XCI4ZTk1OGEzNy04NjUzLWU4MTEtODEyNi03MDEwNmZhYWY4YzFcIixcImFyZWFJZFwiOlwiNlwifSIsIm5iZiI6MCwiZXhwIjoxNTk0OTk3NTY4LCJpYXQiOjE1NjMzNzUxNjgsImlzcyI6Ik91dHN5c3RlbXMiLCJhdWQiOiJNYW5hZ2VBVGVuYW5jeSJ9._fymUyitpMsVbCv3-dibPuUG_TAepZuLxCqyASnZnTk";

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
