using System;

namespace ManageATenancyAPI.Tests.Unit.Services
{
    public class SendTraConfirmationEmailInputModel
    {
        public Guid MeetingId { get; set; }
        public string EmailAddress { get; set; }
        public string TraName { get; set; }
        public string OfficerName { get; set; }
        public string OfficerAddress { get; set; }
    }
}