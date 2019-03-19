using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using Microsoft.AspNetCore.Mvc;
using System;
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

            var checkFrom = officerDetails.LastNewTenancyCheck == null
                ? DateTime.Today
                : officerDetails.LastNewTenancyCheck.Value;

            var response = await _officerService.GetNewTenanciesForHousingOfficer(officerDetails.Id, checkFrom);

            return Ok(HackneyResult<FinaliseETRAMeetingResponse>.Create(response));
        }
    }
}