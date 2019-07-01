using System;
using System.ComponentModel.DataAnnotations;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SignOffMeeting.Boundary
{
    /// <summary>
    /// Input Model for signing off a meeting
    /// </summary>
    public class SignOffMeetingOutputModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        SignOff SignOff { get; set; }
        bool IsSignedOff { get; set; }
    }
}