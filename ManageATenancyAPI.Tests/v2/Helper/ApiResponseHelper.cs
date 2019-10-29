using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Tests.v2.Helper
{
    public static class ApiResponseHelper
    {
        public static T GetResponseType<T>(this IActionResult response) where T : class
        {
            //assert
            response.Should().NotBeNull();
            var result = response as ObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeOfType<T>();

            var outputModel = result.Value as T;
            return outputModel;
        }

        public static T GetOKResponseType<T>(this IActionResult response) where T : class
        {
            //assert
            response.Should().NotBeNull();
            var result = response as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeOfType<T>();

            var outputModel = result.Value as T;
            return outputModel;
        }
    }
}