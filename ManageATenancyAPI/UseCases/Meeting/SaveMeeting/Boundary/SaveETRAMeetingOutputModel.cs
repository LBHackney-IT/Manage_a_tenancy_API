using System;
using System.Collections.Generic;
using ManageATenancyAPI.UseCases.Meeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    /// <summary>
    /// Output model for saving a meeting
    /// </summary>
    public class SaveEtraMeetingOutputModel:IMeetingOutputModel
    {
        /// <summary>
        /// Refers to the TenancyInteractionId in Dynamics 365
        /// </summary>
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<MeetingIssueOutputModel> Issues { get; set; }
        public MeetingAttendees MeetingAttendance { get; set; }
        public SignOff SignOff { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsEmailSent { get; set; }
        public bool IsSignedOff { get; set; }
    }
}