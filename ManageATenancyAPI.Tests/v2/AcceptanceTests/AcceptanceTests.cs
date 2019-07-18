using System;
using System.Collections.Generic;
using ManageATenancyAPI.Services.JWT;
using ManageATenancyAPI.Services.JWT.Models;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace ManageATenancyAPI.Tests.v2.AcceptanceTests
{
    public class AcceptanceTests
    {
        protected static ServiceProvider BuildServiceProvider()
        {
            IServiceCollection collection = new ServiceCollection();
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .Build();

            var startup = new Startup(config, new HostingEnvironment());
            startup.ConfigureServices(collection);
            var serviceProvider = collection.BuildServiceProvider();
            return serviceProvider;
        }

    }

    public static class AcceptanceTestHelper
    {
        public static string SaveMeetingToken = "eyJhbGciOiJIUzI1NiIsImtpZCI6IkhTMjU2IiwidHlwIjoiSldUIn0.eyJzdWIiOiJtaG9sZGVuIiwianRpIjoiIiwibWVldGluZyI6IntcImVzdGF0ZU9mZmljZXJMb2dpbklkXCI6XCIyMDFiYjcyNy1jZTFiLWU4MTEtODExOC03MDEwNmZhYTZhMzFcIixcIm9mZmljZXJJZFwiOlwiMWYxYmI3MjctY2UxYi1lODExLTgxMTgtNzAxMDZmYWE2YTMxXCIsXCJmdWxsTmFtZVwiOlwiTWVnYW4gSG9sZGVuXCIsXCJhcmVhTWFuYWdlcklkXCI6XCI1NTEyYzQ3My05OTUzLWU4MTEtODEyNi03MDEwNmZhYWY4YzFcIixcIm9mZmljZXJQYXRjaElkXCI6XCI4ZTk1OGEzNy04NjUzLWU4MTEtODEyNi03MDEwNmZhYWY4YzFcIixcImFyZWFJZFwiOlwiNlwifSIsIm5iZiI6MCwiZXhwIjoxNTk0OTk3NTY4LCJpYXQiOjE1NjMzNzUxNjgsImlzcyI6Ik91dHN5c3RlbXMiLCJhdWQiOiJNYW5hZ2VBVGVuYW5jeSJ9._fymUyitpMsVbCv3-dibPuUG_TAepZuLxCqyASnZnTk";
        public static void SetTokenHeader(this Controller controller, string token)
        {
            controller.Request.Headers.Clear();
            var headers = new KeyValuePair<string, StringValues>("Authorization", $"Bearer {token}");
            controller.Request.Headers.Add(headers);
        }

        public static void SetSaveMeetingTokenHeader(this Controller controller)
        {
            controller.Request.Headers.Clear();
            var headers = new KeyValuePair<string, StringValues>("Authorization", $"Bearer {SaveMeetingToken}");
            controller.Request.Headers.Add(headers);
        }

        public static IManageATenancyClaims GetManageATenancyClaims(this Controller controller)
        {
            var secret = Environment.GetEnvironmentVariable("HmacSecret");
            return new JWTService().GetManageATenancyClaims(SaveMeetingToken, secret);
        }
    }
}