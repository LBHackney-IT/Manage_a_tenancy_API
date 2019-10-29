using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{

    /// <summary>
    /// Input Model for Save etra meeting use case
    /// </summary>
    public class SaveETRAMeetingInputModel
    {
        [Required]
        public int TRAId { get; set; }
        [Required]
        public string MeetingName { get; set; }
        public MeetingAttendees MeetingAttendance { get; set; }

        public IList<MeetingIssue> Issues { get; set; }

        public SignOff SignOff { get; set; }
    }
}