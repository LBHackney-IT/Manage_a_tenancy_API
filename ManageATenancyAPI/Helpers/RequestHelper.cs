using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Helpers
{
    public static class RequestHelper
    {
        public static CancellationToken GetCancellationToken(this HttpRequest request)
        {
            if(request == null || request.HttpContext == null)
                return CancellationToken.None;
            return request.HttpContext.RequestAborted;
        }
    }
}
