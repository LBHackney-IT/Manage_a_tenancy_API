using System.Collections.Generic;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    public class SaveETRAMeeting: ISaveETRAMeeting
    {
        public int TRAId { get; set; }
        public IMeetingAttendees MeetingAttendance { get; set; }
        public IList<IMeetingIssue> Issues { get; set; }
        public bool IsConfirmed { get; set; }
    }
}