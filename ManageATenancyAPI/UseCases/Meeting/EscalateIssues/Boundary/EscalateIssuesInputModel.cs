using System.Collections.Generic;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.EscalateIssues
{
    public class EscalateIssuesOutputModel
    {
        public IList<EscalateMeetingIssueInputModel> IssuesToEscalate { get; set; }
        public IList<EscalateMeetingIssueInputModel> SuccessfullyEscalatedIssues { get; set; }
        public IList<EscalateMeetingIssueInputModel> FailedToEscalateIssues { get; set; }
    }
}