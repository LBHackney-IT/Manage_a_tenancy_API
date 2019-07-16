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
        
        public Guid Id { get; set; }
        
        public SignOff SignOff { get; set; }
        public bool IsSignedOff { get; set; }
        public bool IsEmailSent { get; set; }
    }
}