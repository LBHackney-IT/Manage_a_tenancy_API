using System.Collections.Generic;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISaveETRAMeeting
    {
        int TRAId { get; set; }
        IMeetingAttendees MeetingAttendance { get; set; }

        IList<IMeetingIssue> Issues { get; set; }


        bool IsConfirmed { get; set; }
    }
}