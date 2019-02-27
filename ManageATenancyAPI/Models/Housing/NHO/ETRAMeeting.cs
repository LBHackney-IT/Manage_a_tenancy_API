using System;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class ETRAMeeting
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public Guid? SignatureId { get; set; }
        public string SignatoryRole { get; set; }
        public Guid? PdfId { get; set; }

        private ETRAMeeting(){ }

        public static ETRAMeeting Create(dynamic crmMeeting)
        {
            return new ETRAMeeting {
                Id = crmMeeting["hackney_tenancymanagementinteractionsid"],
                Name = crmMeeting["hackney_name"],
                CreatedOn = crmMeeting["createdon"],
                ModifiedOn = crmMeeting["modifiedon"],
                ConfirmationDate = crmMeeting["hackney_confirmationdate"],
                SignatureId = crmMeeting["hackney_signaturereference"],
                SignatoryRole = crmMeeting["hackney_signatoryrole"],
                PdfId = crmMeeting["hackney_pdfreference"]
            };
        }
    }
}