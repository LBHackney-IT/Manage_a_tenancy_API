using System;
using Newtonsoft.Json;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    /// <summary>
    /// Class representing a meeting issue after it's been saved
    /// </summary>
    public class MeetingIssueOutputModel:MeetingIssue
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public Guid ServiceRequestId { get; set; }
    }
}