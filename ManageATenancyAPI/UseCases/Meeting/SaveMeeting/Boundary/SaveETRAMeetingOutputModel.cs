using System;

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
    }
}