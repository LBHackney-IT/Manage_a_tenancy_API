using System;
using System.Linq;
using ManageATenancyAPI.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace ManageATenancyAPI.Filters
{
    public class AdminEnabledFilter : ActionFilterAttribute
    {
        private readonly AppConfiguration _config;

        public AdminEnabledFilter(IOptions<AppConfiguration> config)
        {
            _config = config.Value;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_config.EnableAdmin)
            {
                // pass through
            }
            else
            {
                var result = new ObjectResult(new
                {
                    code = StatusCodes.Status401Unauthorized,
                    message = "This is method is disabled",
                });
                result.StatusCode = 401;
                context.Result = result;
            }
        }
        private bool HasClassAttribute(Type clsType, Type attribType)
        {
            if (clsType == null) return false;
            return clsType.GetCustomAttributes(attribType, true).Any() || (clsType.BaseType != null && HasClassAttribute(clsType.BaseType, attribType));
        }
    }
}

