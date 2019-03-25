using ManageATenancyAPI.Helpers;
using System;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class ETRAIssueResponseRequest
    {
        public string IssueId { get; set; }
        public IssueStatus IssueStatus { get; set; }
        public string ServiceArea { get; set; }
        public string ResponseFrom { get; set; }
        public string ResponseText { get; set; }
        public DateTime? ProjectedCompletionDate { get; set; }
        public bool IsPublic { get; set; }
    }
}