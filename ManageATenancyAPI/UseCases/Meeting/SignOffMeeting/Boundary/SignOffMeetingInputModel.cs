using System;
using System.ComponentModel.DataAnnotations;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary
{
    public class SignOffMeetingInputModel
    {
        [Required]
        public Guid MeetingId { get; set; }
        [Required]
        public SignOff SignOff { get; set; }
    }
}