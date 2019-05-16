using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Controllers.Housing.NHO
{
    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class TenancyController : Controller
    {
        private readonly ITenancyService _tenancyService;

        public TenancyController(ITenancyService tenancyService)
        {
            _tenancyService = tenancyService;
        }

        /// <summary>
        /// Gets new tenancies recorded in the CRM since the last run, or in the last day where it hasn't been run before.
        /// </summary>
        /// <returns>A list of new tenancies. This could be an empty list.</returns>
        /// <response code="200">Successfully returned any new tenancies</response>
        [Route("new")]
        [HttpGet]
        public async Task<ActionResult<HackneyResult<IList<NewTenancyResponse>>>> GetNewTenancies()
        {
            var response = await _tenancyService.GetNewTenancies();

            return Ok(HackneyResult<IList<NewTenancyResponse>>.Create(response));
        }
    }
}