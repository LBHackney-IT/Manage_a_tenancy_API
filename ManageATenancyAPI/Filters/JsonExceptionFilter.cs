using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManageATenancyAPI.Filters
{
    public class JsonExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var result = new ObjectResult(HackneyResult<ApiErrorMessage>.Create(new
                ApiErrorMessage()
            {
                userMessage = "A server error occurred.",
                developerMessage = context.Exception.Message
            }));
            result.StatusCode = 500;
            context.Result = result;
        }
    }
}
