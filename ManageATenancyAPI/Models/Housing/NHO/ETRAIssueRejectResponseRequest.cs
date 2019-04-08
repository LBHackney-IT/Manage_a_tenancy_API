using System;
using System.ComponentModel.DataAnnotations;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class ETRAIssueRejectResponseRequest
    {
        [Required]
        public string ResponderName { get; set; }
        [Required]
        public string ResponseText { get; set; }
        [Required]
        public Guid IssueIncidentId { get; set; } //CRM incident Guid
        [Required]
        public Guid AnnotationSubjectId { get; set; }
    }
}