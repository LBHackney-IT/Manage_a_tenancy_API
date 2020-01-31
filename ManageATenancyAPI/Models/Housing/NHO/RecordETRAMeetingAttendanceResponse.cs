using System;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class RecordETRAMeetingAttendanceResponse
    {
        public string MeetingId { get; set; }
        public bool Recorded { get; set; }

        public string Councillors { get; set; }
        public string OtherCouncilStaff { get; set; }
        public int? TotalAttendees { get; set; }
    }
}