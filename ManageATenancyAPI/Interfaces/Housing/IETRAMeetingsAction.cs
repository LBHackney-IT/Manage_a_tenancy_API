using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Interfaces.Housing
{
    public interface IETRAMeetingsAction
    {
        Task<ETRAMeeting> GetMeeting(string id);
        Task<HackneyResult<JObject>> CreateETRAMeeting(ETRAIssue meetingInfo);
        Task<object> GetETRAIssuesByTRAorETRAMeeting(string id, bool retrieveIssuesPerMeeting);
        Task<JObject> UpdateIssue(UpdateETRAIssue etraIssueToBeUpdated);
        Task<FinaliseETRAMeetingResponse> FinaliseMeeting(string id, FinaliseETRAMeetingRequest request);
    }
}
