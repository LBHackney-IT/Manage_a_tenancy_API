using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;
using ManageATenancyAPI.Models.Housing.NHO;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Interfaces.Housing
{
    public interface IETRAMeetingsAction
    {
        Task<ETRAMeeting> GetMeetingAsync(string id);
        Task<GetEtraMeetingOutputModel> GetMeetingV2Async(Guid id, CancellationToken cancellationToken);
        Task<CreateETRAMeetingActionResponse> CreateETRAMeeting(ETRAIssue meetingInfo);
        Task<object> GetETRAIssuesByTRAorETRAMeeting(string id, bool retrieveIssuesPerMeeting);
        Task<IList<MeetingIssueOutputModel>> GetETRAIssuesForMeeting(Guid id, CancellationToken cancellationToken);

        Task<ETRAUpdateResponse> UpdateIssue(UpdateETRAIssue etraIssueToBeUpdated);
        Task<FinaliseETRAMeetingResponse> FinaliseMeeting(string id, FinaliseETRAMeetingRequest request);
        Task<RecordETRAMeetingAttendanceResponse> RecordETRAMeetingAttendance(string id, RecordETRAMeetingAttendanceRequest request);
        Task<ETRAUpdateResponse> AddETRAIssueResponse(string id, ETRAIssueResponseRequest request);
        Task<ETRAUpdateResponse> RejectETRAIssueResponse(string id, ETRAIssueRejectResponseRequest request);
        Task<IEnumerable<ETRAMeeting>> GetETRAMeetingsForTRAId(string id);
        Task<IncidentClosedResponse> CloseIncident(string closingNotes, Guid incidentId);
        Task<GetAllEtraIssuesThatNeedEscalatingOutputModel> GetAllEtraIssuesThatNeedEscalatingAsync(DateTime fromDate, CancellationToken cancellationToken);
        Task<bool> EscalateIssue(MeetingIssueOutputModel issue, CancellationToken cancellationToken);
    }
}