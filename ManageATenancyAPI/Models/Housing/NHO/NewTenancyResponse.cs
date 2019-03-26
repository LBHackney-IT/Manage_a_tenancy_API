using System;
using System.Collections.Generic;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class NewTenancyResponse
    {
        public string AccountId { get; set; }
        public string OfficerId { get; set; }
        public string OfficerName { get; set; }
        public DateTime AccountCreatedOn { get; set; }
        public string HousingTenure { get; set; }
        public string PatchId { get; set; }
        public string AreaId { get; set; }
        public string ManagerId { get; set; }
        public string HouseholdId { get; set; }
        public string TagReference { get; set; }

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
        public string ContactId { get; set; }
        public bool Responsible { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}