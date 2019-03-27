using ManageATenancyAPI.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class ETRAIssueResponseRequest
    {
        [Required]
        public string IssueId { get; set; }

        [Required]
        public IssueStatus IssueStatus { get; set; }

        [Required]
        public string ServiceArea { get; set; }

        [Required]
        public string ResponseFrom { get; set; }

        [Required]
        public string ResponseText { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        public DateTime? ProjectedCompletionDate { get; set; }
    }
}