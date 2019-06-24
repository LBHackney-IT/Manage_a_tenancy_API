namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    /// <summary>
    /// Interface representing a meeting issue
    /// </summary>
    public interface IMeetingIssue
    {
        /// <summary>
        /// 
        /// </summary>
        string IssueTypeId { get; set; }
        string IssueLocationName { get; set; }
        string IssueNote { get; set; }
    }
}