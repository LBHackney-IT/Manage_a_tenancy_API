using System.Collections.Generic;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.EscalateIssues
{
    public class EscalateIssuesOutputModel
    {
        public IList<MeetingIssueOutputModel> IssuesToEscalate { get; set; }
        public IList<MeetingIssueOutputModel> SuccessfullyEscalatedIssues { get; set; }
        public IList<MeetingIssueOutputModel> FailedToEscalateIssues { get; set; }
    }
}