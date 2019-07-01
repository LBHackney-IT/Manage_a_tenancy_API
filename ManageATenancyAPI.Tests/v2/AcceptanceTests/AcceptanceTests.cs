using System.Collections.Generic;
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
                .AddJsonFile("appsettings.json")
                .Build();

            var startup = new Startup(config, new HostingEnvironment());
            startup.ConfigureServices(collection);
            var serviceProvider = collection.BuildServiceProvider();
            return serviceProvider;
        }

    }

    public static class AcceptanceTestHelper
    {
        private static string _saveMeetingToken = "eyJhbGciOiJIUzI1NiIsImtpZCI6IkhTMjU2IiwidHlwIjoiSldUIn0.eyJzdWIiOiJtaG9sZGVuIiwianRpIjoiIiwiQ3JlYXRlIG1lZXRpbmciOiJ7XCJlc3RhdGVPZmZpY2VyTG9naW5JZFwiOlwiMWYxYmI3MjctY2UxYi1lODExLTgxMTgtNzAxMDZmYWE2YTMxXCIsXCJvZmZpY2VySWRcIjpcIjFmMWJiNzI3LWNlMWItZTgxMS04MTE4LTcwMTA2ZmFhNmEzMVwiLFwidXNlcm5hbWVcIjpcIm1ob2xkZW5cIixcImZ1bGxOYW1lXCI6XCJNZWdhbiBIb2xkZW5cIixcImFyZWFNYW5hZ2VySWRcIjpcIjU1MTJjNDczLTk5NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwib2ZmaWNlclBhdGNoSWRcIjpcIjhlOTU4YTM3LTg2NTMtZTgxMS04MTI2LTcwMTA2ZmFhZjhjMVwiLFwiYXJlYUlkXCI6XCI2XCJ9IiwibmJmIjowLCJleHAiOjE1OTMwNzY2MjYsImlhdCI6MTU2MTQ1NDIyNiwiaXNzIjoiT3V0c3lzdGVtcyIsImF1ZCI6Ik1hbmFnZUFUZW5hbmN5In0.d7e_bDz1JnZdXjDASng67HWmC7s466lfQEDK-weyXCQ";
        public static void SetTokenHeader(this Controller controller, string token)
        {
            controller.Request.Headers.Clear();
            var headers = new KeyValuePair<string, StringValues>("Authorization", $"Bearer {token}");
            controller.Request.Headers.Add(headers);
        }

        public static void SetSaveMeetingTokenHeader(this Controller controller)
        {
            controller.Request.Headers.Clear();
            var headers = new KeyValuePair<string, StringValues>("Authorization", $"Bearer {_saveMeetingToken}");
            controller.Request.Headers.Add(headers);
        }
    }
}