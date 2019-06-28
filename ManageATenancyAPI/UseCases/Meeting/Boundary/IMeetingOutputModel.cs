using System;
using System.Collections.Generic;
using ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary;

namespace ManageATenancyAPI.UseCases.Meeting.Boundary
{
    /// <summary>
    /// Interface Output model for a meeting
    /// </summary>
    public interface IMeetingOutputModel
    {
        /// <summary>
        /// Refers to the TenancyInteractionId in Dynamics 365
        /// </summary>
        Guid MeetingId { get; set; }

        IList<MeetingIssueOutputModel> Issues { get; set; }
        SignOff SignOff { get; set; }
        bool IsSignedOff { get; set; }
    }
}