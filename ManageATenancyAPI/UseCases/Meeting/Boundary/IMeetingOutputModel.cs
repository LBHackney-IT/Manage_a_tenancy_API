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
        Guid Id { get; set; }
        string Name { get; set; }
        IList<MeetingIssueOutputModel> Issues { get; set; }
        MeetingAttendees MeetingAttendance { get; set; }
        SignOff SignOff { get; set; }
        DateTime CreatedOn { get; set; }
        bool IsEmailSent { get; set; }
        bool IsSignedOff { get; set; }
    }
}