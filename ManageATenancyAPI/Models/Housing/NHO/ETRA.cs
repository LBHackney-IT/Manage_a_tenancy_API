﻿using System;
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
}
