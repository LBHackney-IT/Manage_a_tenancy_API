using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveETRAMeetingInputModel
    {
        [Required]
        public int TRAId { get; set; }
        public MeetingAttendees MeetingAttendance { get; set; }

        public IList<MeetingIssue> Issues { get; set; }

        public SignOff SignOff { get; set; }
    }
}