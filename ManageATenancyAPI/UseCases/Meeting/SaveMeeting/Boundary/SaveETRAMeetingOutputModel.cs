using System;
using System.Collections.Generic;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    /// <summary>
    /// Output model for saving a meeting
    /// </summary>
    public class SaveETRAMeetingOutputModel
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