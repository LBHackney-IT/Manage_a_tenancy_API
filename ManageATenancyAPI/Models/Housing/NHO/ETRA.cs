using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class ETRA
    {
     
        [Required]
        public string estateOfficerId { get; set; }
        public string subject { get; set; }
        [Required]
        public string estateOfficerName { get; set; }
        [Required]
        public string officerPatchId { get; set; }
        [Required]
        public string areaName { get; set; }
        [Required]
        public string managerId { get; set; }
        public CRMServiceRequest ServiceRequest { get; set; }
        public string processType { get; set; }
        [Required]
        public string TRAId { get; set; }
    }

    public class ETRAIssue
    {
        [Required]
        public string estateOfficerId { get; set; }
        public string subject { get; set; }
        [Required]
        public string estateOfficerName { get; set; }
        [Required]
        public string officerPatchId { get; set; }
        [Required]
        public string areaName { get; set; }
        [Required]
        public string managerId { get; set; }
        public CRMServiceRequest ServiceRequest { get; set; }
        [Required]
        public string processType { get; set; }
        [Required]
        public string TRAId { get; set; }
       
        public string issueLocation { get; set; }
       
        public string parentInteractionId { get; set; }
        [Required]
        public string natureOfEnquiry { get; set; }
        [Required]
        public string enquirySubject { get; set; }
    }

    public class UpdateETRAIssue
    {
        [Required]
        public string estateOfficerId { get; set; }
        [Required]
        public string estateOfficerName { get; set; }
        [Required]
        public string note { get; set; }
        public string serviceArea { get; set; }
        public string issueStage { get; set; }
        public Guid annotationId { get; set; }
        [Required]
        public Guid issueInteractionId { get; set; } //CRM interaction Guid 
        [Required]
        public Guid issueIncidentId { get; set; } //CRM incident Guid 
        [Required]
        public bool isNewNote { get; set; } //true if note is new, false if note needs to be updated
        [Required]
        public bool issueIsToBeDeleted { get; set; }
        public Guid AnnotationSubjectId { get; set; }

    }

    public class FinaliseETRAMeetingRequest
    {
        public string Role { get; set; }
        public Guid SignatureId { get; set; }
    }

    public class FinaliseETRAMeetingResponse
    {
        public string Id { get; set; }
        public bool IsFinalised { get; set; }
    }

}
