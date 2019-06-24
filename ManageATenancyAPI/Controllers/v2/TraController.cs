using System.Diagnostics;
using System.Threading.Tasks;
using ManageATenancyAPI.Gateways.GetAllTRAs;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.v2
{
    [Produces("application/json")]
    [Route("v1/tra")]
    public class TraController : Controller
    {
        public TraController()
        {

        }

        /// <summary>
        /// Creates an ETRA meeting
        /// </summary>
        /// <returns>A JSON object for a successfully created ETRA meeting request</returns>
        /// <response code="201">A successfully created ETRA meeting request</response>
        [HttpPost]
        public async Task<> Get()
        {

        }
    }
}