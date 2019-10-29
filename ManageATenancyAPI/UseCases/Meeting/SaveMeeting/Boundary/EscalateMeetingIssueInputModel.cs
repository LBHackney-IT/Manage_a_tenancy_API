using System;
using Newtonsoft.Json;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    public class EscalateMeetingIssueInputModel : MeetingIssue
    {
        public Guid Id { get; set; }

        public Guid ServiceRequestId { get; set; }

        public string AreaId { get; set; }

        public bool ServiceOfficerEmailSent { get; set; }
        public bool ServiceAreaManagerEmailSent { get; set; }
        public bool AreaHousingManagerEmailSent { get; set; }
        public DateTime? DueDate { get; set; }

        public int TraId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string TicketNumber { get; set; }
        public string HousingOfficerName { get; set; }
        public string IssueName { get; set; }
    }
}