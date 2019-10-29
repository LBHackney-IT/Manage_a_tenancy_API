using System;
using System.ComponentModel.DataAnnotations;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.EscalateIssues
{
    public class TRAIssue
    {
        public MeetingIssueOutputModel Issue { get; set; }

        public TRAIssueServiceArea ServiceArea { get; set; }
    }
}