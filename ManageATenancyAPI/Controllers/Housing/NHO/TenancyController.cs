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

        [Route("new")]
        [HttpGet]
        public async Task<ActionResult<HackneyResult<IList<NewTenancyResponse>>>> GetNewTenancies()
        {
            var response = await _tenancyService.GetNewTenancies();

            return Ok(HackneyResult<IList<NewTenancyResponse>>.Create(response));
        }
    }
}