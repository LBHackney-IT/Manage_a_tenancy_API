using ManageATenancyAPI.Services;
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
            _token = "eyJhbGciOiJIUzI1NiIsImtpZCI6IkhTMjU2IiwidHlwIjoiSldUIn0.eyJzdWIiOiJtaG9sZGVuIiwianRpIjoiIiwiQ3JlYXRlIG1lZXRpbmciOiJ7XCJlc3RhdGVPZmZpY2VyTG9naW5JZFwiOlwiMWYxYmI3MjctY2UxYi1lODExLTgxMTgtNzAxMDZmYWE2YTMxXCIsXCJvZmZpY2VySWRcIjpcIjFmMWJiNzI3LWNlMWItZTgxMS04MTE4LTcwMTA2ZmFhNmEzMVwiLFwidXNlcm5hbWVcIjpcIm1ob2xkZW5cIixcImZ1bGxOYW1lXCI6XCJNZWdhbiBIb2xkZW5cIixcImFyZWFNYW5hZ2VySWRcIjpcIjU1MTJjNDczLTk5NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwib2ZmaWNlclBhdGNoSWRcIjpcIjhlOTU4YTM3LTg2NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwiYXJlYUlkXCI6XCI2XCJ9IiwibmJmIjowLCJleHAiOjE1OTMwNzY2MjYsImlhdCI6MTU2MTQ1NDIyNiwiaXNzIjoiT3V0c3lzdGVtcyIsImF1ZCI6Ik1hbmFnZUFUZW5hbmN5In0.d7e_bDz1JnZdXjDASng67HWmC7s466lfQEDK-weyXCQ";
            _secret = "eyJhbGciOiJIUzI1NiIsImtpZCI6Ikw0d2Y4QVJXeWdEUl9rMlRvenI0b2xoSkNuaGczVVYwMWVHVFE3WDJsLVkiLCJ0eXAiOiJKV1QifQ";
            _classUnderTest = new JWTService();
        }

        [Fact]
        public void can_decrypt_token()
        {
            //arrange

            //act
            var claims = _classUnderTest.GetClaims(_token, _secret);
            //assert
            claims.Should().NotBeNull();
        }

        [Fact]
        public void can_get_manage_a_tenancy_claims()
        {
            //arrange

            //act
            var claims = _classUnderTest.GetManageATenancyClaims(_token, _secret);
            //assert
            claims.Should().NotBeNull();
        }
    }
}
