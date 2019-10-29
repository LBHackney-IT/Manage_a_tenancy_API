using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.Gateways.EscalateIssue
{
    public class EscalateIssueInputModel
    {
        public EscalateMeetingIssueInputModel Issue { get; set; }
    }
}