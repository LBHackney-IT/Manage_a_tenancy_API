namespace ManageATenancyAPI.UseCases.Meeting.EscalateIssues
{
    public class TRAIssueServiceArea
    {
        public string IssueId { get; set; }
        public string IssueType { get; set; }

        public string ServiceAreaOfficer { get; set; }
        public string ServiceAreaOfficerEmail { get; set; }

        public string ServiceAreaManager { get; set; }
        public string ServiceAreaManagerEmail { get; set; }
    }
}