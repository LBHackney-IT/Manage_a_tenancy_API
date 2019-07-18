using System.Collections.Generic;
using ManageATenancyAPI.UseCases.Meeting.EscalateIssues;
using ManageATenancyAPI.UseCases.Meeting.GetMeeting;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.Interfaces.Housing
{
    public class GetAllEtraIssuesThatNeedEscalatingOutputModel
    {
        public IList<MeetingIssueOutputModel> IssuesThatNeedEscalating { get; set; }
    }
}