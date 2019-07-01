using System;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary
{
    public class SignOffMeetingInputModel
    {
        public Guid MeetingId { get; set; }
        public SignOff SignOff { get; set; }
    }
}