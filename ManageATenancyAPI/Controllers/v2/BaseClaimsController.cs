using System;
using System.Globalization;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Services.JWT.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.v2
{
    public class BaseClaimsController : Controller
    {
        private readonly IJWTService _jwtService;
        private string _authorization;
        protected IManageATenancyClaims Claims { get; set; }
        public BaseClaimsController(IJWTService jwtService)
        {
            _jwtService = jwtService;
            _authorization = "Authorization";
        }

        protected IManageATenancyClaims GetHousingOfficerClaims()
        {
            if (Request.Headers.ContainsKey(_authorization))
            {
                var authString = Request.Headers[_authorization].ToString();
                authString = authString.Replace("bearer ", "", true, CultureInfo.InvariantCulture);

                Claims = _jwtService.GetManageATenancyClaims(authString, Environment.GetEnvironmentVariable("HmacSecret"));
            }

            return Claims;
        }

        protected IMeetingClaims GetMeetingClaims()
        {
            IMeetingClaims claims = null;
            if (Request.Headers.ContainsKey(_authorization))
            {
                var token = Request.Headers[_authorization].ToString();
                token = token.Replace("bearer ", "", true, CultureInfo.InvariantCulture);

                claims = _jwtService.GetMeetingIdClaims(token, Environment.GetEnvironmentVariable("HmacSecret"));
            }

            return claims;
        }
    }
}