using System;
using System.Collections.Generic;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class NewTenancyResponse
    {
        public string AccountId { get; set; }
        public DateTime AccountCreatedOn { get; set; }
        public string NeighbourhoodOffice { get; set; }
        public string EstateAddress { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostCode { get; set; }
        public string FullAddress { get; set; }
        public IList<NewTenancyContact> Contacts { get; set; }
    }

    public class NewTenancyContact
    {
        public bool Responsible { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}