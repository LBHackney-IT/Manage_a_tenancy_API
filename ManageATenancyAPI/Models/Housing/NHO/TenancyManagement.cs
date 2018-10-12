using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class TenancyManagement
    {
        public string interactionId { get; set; }
        public string contactId { get; set; }
        public string enquirySubject { get; set; }
        public string estateOfficerId { get; set; }
        public string subject { get; set; }
        public string adviceGiven { get; set; }
        public string estateOffice { get; set; }
        public string source { get; set; }
        public string natureofEnquiry { get; set; }
        public string estateOfficerName { get; set; }
        public string officerPatchId { get; set; }
        public string areaName { get; set; }
        public string managerId { get; set; }
        public bool assignedToPatch { get; set; }
        public bool assignedToManager { get; set; }
        public bool transferred { get; set; }
        public CRMServiceRequest ServiceRequest { get; set; }
        public int status { get; set; } // status=0 is sent in case we need to close the call status=1 if we need to update the call
        public string parentInteractionId { get; set; }
        public string processType { get; set; }
        public string householdId { get; set; }
        public int processStage { get; set; }
        public int? reasonForStartingProcess { get; set; }
    }
}
