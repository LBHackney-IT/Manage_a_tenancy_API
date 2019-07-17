using System;
using System.ComponentModel.DataAnnotations;

namespace ManageATenancyAPI.UseCases.Meeting.EscalateIssues
{
    public class TRAIssue
    {
        public Guid Id { get; set; }

        [MaxLength(32)]
        public string IssueId { get; set; }

        [MaxLength(64)]
        public string IssueType { get; set; }

        [MaxLength(128)]
        public string Location { get; set; }

        private TRAIssueServiceArea ServiceArea { get; set; }
    }
}