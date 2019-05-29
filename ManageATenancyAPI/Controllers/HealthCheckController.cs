using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers
{
    [Route("api/v1/healthcheck")]
    [Produces("application/json")]
    public class HealthCheckController : Controller
    {
        [HttpGet]
        [Route("ping")]
        [ProducesResponseType(typeof(Dictionary<string, bool>), 200)]
        public IActionResult HealthCheck()
        {
            var result = new Dictionary<string, bool> {{"success", true}};

            return Ok(result);
        }
    }
}