using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{


    public class ETRAIssue
    {
        //ETRA MEETING
        [Required]
        public string estateOfficerId { get; set; }
        /// <summary>
        /// CRM ENtity Subjects.subjectid 72873b3e-5255-e911-a97a-002248072cc3
        /// Generic subject id is used to Creating Meeting = c1f72d01-28dc-e711-8115-70106faa6a11" (dev)
        /// </summary>
        public string subject { get; set; }
        [Required]
        public string estateOfficerName { get; set; }
        /// <summary>
        /// Patch - ID - Can be swapped , still links to AREA ID
        /// </summary>
        [Required]
        public string officerPatchId { get; set; }
        /// <summary>
        /// AreaId - AREA ID Taken from Login information
        /// </summary>
        [Required]
        public string areaName { get; set; }
        [Required]
        public string managerId { get; set; }
        public CRMServiceRequest ServiceRequest { get; set; }

        /// <summary>
        /// ALWAYS 3
        /// </summary>
        [Required]
        public string processType { get; set; }

        [Required]
        public string TRAId { get; set; }

        //ETRA ISSUE
        /// <summary>
        /// Name of TRA block within an estate or the estate itself
        /// </summary>
        public string issueLocation { get; set; }
       

        /// <summary>
        /// Tenancy Management Interaction Id for Meeting
        /// </summary>
        public string parentInteractionId { get; set; }

        /// <summary>
        /// ALWAYS 28 - ETRA
        /// </summary>
        [Required]
        public string natureOfEnquiry { get; set; }

        /// Issue Type Id - 10000111 - NOT NAME
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
        public string IssueLocation { get; set; }
        public string EnquirySubject { get; set; }
        public Guid? AnnotationSubjectId { get; set; }
        public Guid? PDFId { get; set; }
        public Guid? SignatureId { get; set; }
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
