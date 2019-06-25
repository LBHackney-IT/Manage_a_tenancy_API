namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    /// <summary>
    /// Interface representing a meeting issue
    /// </summary>
    public class MeetingIssue
    {
        /// <summary>
        ///
        /// </summary>
        public string IssueTypeId { get; set; }
        public string IssueLocationName { get; set; }
        public string IssueNote { get; set; }
    }
}