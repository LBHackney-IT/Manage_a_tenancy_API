using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.v2
{
    [Produces("application/json")]
    [Route("v1/etra/meeting/issue-types")]
    public class ETRAMeetingIssueTypesController : Controller
    {
        private readonly IGetEtraMeetingIssueTypesUseCase _getEtraMeetingIssueTypesUseCase;

        public ETRAMeetingIssueTypesController(IGetEtraMeetingIssueTypesUseCase getEtraMeetingIssueTypesUseCase)
        {
            _getEtraMeetingIssueTypesUseCase = getEtraMeetingIssueTypesUseCase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Get()
        {
            
        }
    }
}