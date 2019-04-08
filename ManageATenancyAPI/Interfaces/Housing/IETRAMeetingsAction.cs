using System.Collections.Generic;
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
        Task<ETRAUpdateResponse> UpdateIssue(UpdateETRAIssue etraIssueToBeUpdated);
        Task<FinaliseETRAMeetingResponse> FinaliseMeeting(string id, FinaliseETRAMeetingRequest request);
        Task<RecordETRAMeetingAttendanceResponse> RecordETRAMeetingAttendance(string id, RecordETRAMeetingAttendanceRequest request);
        Task<ETRAUpdateResponse> AddETRAIssueResponse(string id, ETRAIssueResponseRequest request);
        Task<ETRAUpdateResponse> RejectETRAIssueResponse(string id, ETRAIssueRejectResponseRequest request);
        Task<IEnumerable<ETRAMeeting>> GetETRAMeetingsForTRAId(string id);
    }
}