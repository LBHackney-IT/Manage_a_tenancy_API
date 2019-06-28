using System;
using System.Collections.Generic;
using ManageATenancyAPI.UseCases.Meeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    /// <summary>
    /// Output model for saving a meeting
    /// </summary>
    public class SaveEtraMeetingOutputModelOutputModel:IMeetingOutputModel
    {
        /// <summary>
        /// Refers to the TenancyInteractionId in Dynamics 365
        /// </summary>
        public Guid MeetingId { get; set; }

        public IList<MeetingIssueOutputModel> Issues { get; set; }
        public SignOff SignOff { get; set; }
        public bool IsSignedOff { get; set; }
    }
}