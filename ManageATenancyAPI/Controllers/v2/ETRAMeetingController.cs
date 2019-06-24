using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.UseCases;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Microsoft.AspNetCore.Mvc;


namespace ManageATenancyAPI.Controllers.v2
{
    [Produces("application/json")]
    [Route("v1/etra/meeting")]
    public class ETRAMeetingController : Controller
    {
        

        public ETRAMeetingController()
        {
            
        }

        ///// <summary>
        ///// Creates an ETRA meeting
        ///// </summary>
        ///// <returns>A JSON object for a successfully created ETRA meeting request</returns>
        ///// <response code="201">A successfully created ETRA meeting request</response>
        //[HttpPost]
        //public async Task<> Post([FromBody] ICreateETRAMeeting etraMeeting)
        //{
        //    return null;
        //}
    }


}