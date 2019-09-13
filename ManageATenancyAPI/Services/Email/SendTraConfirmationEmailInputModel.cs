using System;

namespace ManageATenancyAPI.Services.Email
{
    public class SendTraConfirmationEmailInputModel
    {
        public Guid MeetingId { get; set; }
        public int TraId { get; set; }
        public string OfficerName { get; set; }
        public string OfficerAddress { get; set; }
        public bool IsMeetingSignedOff { get; set; }
    }
}