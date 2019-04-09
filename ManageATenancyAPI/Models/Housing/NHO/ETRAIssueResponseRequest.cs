using System;
using System.ComponentModel.DataAnnotations;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class ETRAIssueResponseRequest
    {
        [Required]
        public string ResponderName { get; set; }
        [Required]
        public string ResponseText { get; set; }
        [Required]
        public string ServiceAreaName { get; set; }
        [Required]
        public int ServiceAreaId { get; set; }
        [Required]
        public string IssueStage { get; set; }
        [Required]
        public Guid IssueIncidentId { get; set; } //CRM incident Guid
        [Required]
        public Guid AnnotationSubjectId { get; set; }
        public DateTime? ProjectedCompletionDate { get; set; }
    }
}