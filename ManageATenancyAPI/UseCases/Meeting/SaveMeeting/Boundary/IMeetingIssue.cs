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
        public IssueType IssueType { get; set; }
        public Location Location { get; set; }
        public string Notes { get; set; }
    }

    public class IssueType
    {
        public string IssueId{ get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
    }
}