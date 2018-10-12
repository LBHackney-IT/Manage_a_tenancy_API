using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManageATenancyAPI.Models
{
    public class CRMServiceRequest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ContactId { get; set; }
        public string ParentCaseId { get; set; }
        public string Subject { get; set; }
        public string CreatedDate { get; set; }
        public string EnquiryType { get; set; }
        public string TicketNumber { get; set; }
        public bool RequestCallback { get; set; }
        public bool Transferred { get; set; }
        public string CreatedBy { get; set; }

        public List<CRMServiceRequest> ChildRequests { get; set; }
    }
}