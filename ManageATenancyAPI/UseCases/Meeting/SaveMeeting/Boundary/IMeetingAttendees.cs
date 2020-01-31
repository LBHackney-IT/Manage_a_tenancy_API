﻿using System.ComponentModel.DataAnnotations;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    /// <summary>
    /// Interface describing attendees of a meeting
    /// </summary>
    public class MeetingAttendees
    {
        /// <summary>
        /// Optional ideally comma separated list of Councillors as a record of who attended
        /// </summary>
        public string Councillors { get; set; }

        /// <summary>
        /// Optional ideally comma seperated list of Hackney staff as a record of who attended 
        /// </summary>
        public string HackneyStaff { get; set; }

        /// <summary>
        /// Number of attendees to the meeting that aren't Councillors or Hackney staff, must be greater than 1
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? Attendees { get; set; }

    }
}