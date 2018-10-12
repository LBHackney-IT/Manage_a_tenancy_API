using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Newtonsoft.Json;


namespace ManageATenancyAPI.Models.Housing
{
    public class AccountsAndAddress
    {
        public string parisReferenceNumber { get; set; }
        public string postcode { get; set; }
        public string address { get; set; }
        public int? addressType { get; set; } //1 = main address, 2=correspondence address

    }

    public class AccountDetails
    {
        public string propertyReferenceNumber { get; set; }
        public string benefit { get; set; }
        public string tagReferenceNumber { get; set; }
        public string paymentReferenceNumber { get; set; }
        public string accountid { get; set; }
        public string currentBalance { get; set; }
        public string rent { get; set; }
        public string housingReferenceNumber { get; set; }
        public string directdebit { get; set; }
        public string tenancyStartDate { get; set; }
        public string agreementType { get; set; }
        public bool isAgreementTerminated { get; set; }
        public string tenuretype { get; set; }
        public string accountType { get; set; }
    }

    public class AccountsAndNotifications
    {
        public string currentBalance { get; set; }
        public string telephone { get; set; }
        public bool areNotificationsOn { get; set; }
        public string paymentAgreementId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string paymentAgreementEndDate { get; set; }

    }

    public class AccountObject<T>
    {
        public List<T> value { get; set; }
        public HttpStatusCode ApiStatusCode { get; set; }
    }
}