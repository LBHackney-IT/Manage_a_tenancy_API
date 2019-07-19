using System;
using Newtonsoft.Json;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    public class EscalateMeetingIssueInputModel : MeetingIssue
    {
        public Guid Id { get; set; }

        public Guid ServiceRequestId { get; set; }

        public string AreaId { get; set; }
    }
}