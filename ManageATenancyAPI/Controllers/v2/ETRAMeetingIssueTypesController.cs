using System.Collections.Generic;
using System.Threading.Tasks;
using ManageATenancyAPI.UseCases.IssueType.GetAllIssueTypes;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting;
using Microsoft.AspNetCore.Mvc;

namespace ManageATenancyAPI.Controllers.v2
{
    [Produces("application/json")]
    [Route("v1/etra/meeting/issue-types")]
    public class ETRAMeetingIssueTypesController : Controller
    {
        private readonly IGetAllIssueTypesUseCase _getAllIssueTypesUse;

        public ETRAMeetingIssueTypesController(IGetAllIssueTypesUseCase getAllIssueTypesUse)
        {
            _getAllIssueTypesUse = getAllIssueTypesUse;
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

        public class GetAllIssueTypesOutputModel
        {
            public IList<MeetingIssueType> Issues { get; set; }
        }

        public class MeetingIssueType
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}