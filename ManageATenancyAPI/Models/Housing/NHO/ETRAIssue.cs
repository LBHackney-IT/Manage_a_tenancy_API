using System;
using System.Collections.Generic;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class ETRAIssue : ETRAIssueRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        private ETRAIssue() { }

        public static ETRAIssue Create(dynamic crmIssue)
        {
            return new ETRAIssue
            {
                Id = crmIssue["hackney_tenancymanagementinteractionsid"],
                Name = crmIssue["hackney_name"],
                CreatedOn = crmIssue["createdon"],
                ModifiedOn = crmIssue["modifiedon"],
                areaName = crmIssue["hackney_areaname"],
                enquirySubject = crmIssue["hackney_enquirysubject"],
                estateOfficerId = crmIssue["_hackney_estateofficer_createdbyid_value"],
                //estateOfficerName = crmIssue[""],
                issueLocation = crmIssue["hackney_issuelocation"],
                managerId = crmIssue["_hackney_managerpropertypatchid_value"],
                natureOfEnquiry = crmIssue["hackney_natureofenquiry"],
                officerPatchId = crmIssue["_hackney_estateofficerpatchid_value"],
                parentInteractionId = crmIssue["_hackney_parent_interactionid_value"],
                processType = crmIssue["hackney_processtype"],
                subject = crmIssue["_hackney_subjectid_value"],
                TRAId = crmIssue["hackney_traid"],
                ServiceRequest = new CRMServiceRequest
                {
                    ContactId = crmIssue["_hackney_contactid_value"],
                    CreatedBy = crmIssue["_createdby_value"],
                    Description = crmIssue["hackney_name"],
                    EnquiryType = crmIssue["hackney_natureofenquiry"],
                    //CreatedDate = crmIssue[""],
                    //Id = crmIssue[""],
                    //ParentCaseId = crmIssue[""],
                    //RequestCallback = crmIssue[""],
                    //Subject = crmIssue[""],
                    //TicketNumber = crmIssue[""],
                    //Title = crmIssue[""],
                    Transferred = crmIssue["hackney_transferred"],
                    ChildRequests = new List<CRMServiceRequest>()
                }
            };
        }
    }
}