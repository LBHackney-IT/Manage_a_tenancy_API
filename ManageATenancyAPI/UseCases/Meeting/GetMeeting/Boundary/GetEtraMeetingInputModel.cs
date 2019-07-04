using System;

namespace ManageATenancyAPI.UseCases.Meeting.GetMeeting
{
    /// <summary>
    /// InputModel for getting a meeting
    /// </summary>
    public class GetEtraMeetingInputModel
    {
        public Guid MeetingId { get; set; }
    }
}