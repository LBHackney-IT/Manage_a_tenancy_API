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
    public class OfficerController : Controller
    {
        private readonly IOfficerService _officerService;

        public OfficerController(IOfficerService officerService)
        {
            _officerService = officerService;
        }

        [Route("{emailAddress}/new-tenancies")]
        [HttpGet]
        public async Task<ActionResult<HackneyResult<IList<NewTenancyResponse>>>> GetNewTenanciesForHousingOfficer(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return BadRequest();

            var officerDetails = await _officerService.GetOfficerDetails(emailAddress);

            if (officerDetails == null)
                return NotFound();

            var response = await _officerService.GetNewTenanciesForHousingOfficer(officerDetails);

            return Ok(HackneyResult<IList<NewTenancyResponse>>.Create(response));
        }
    }
}