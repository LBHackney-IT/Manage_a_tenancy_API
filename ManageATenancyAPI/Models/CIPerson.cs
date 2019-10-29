using System;

namespace ManageATenancyAPI.Models
{
    public class CIPerson
    {
        public string ID { get; set; }
        public string HackneyhomesId { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string PostCode { get; set; }
        public string SystemName { get; set; }
        public string LARN { get; set; }
        public string UPRN { get; set; }
        public string USN { get; set; }
        public string FullAddressSearch { get; set; }
        public string FullAddressDisplay { get; set; }
        public bool? CautionaryAlert { get; set; }
        public bool? PropertyCautionaryAlert { get; set; }
        public Guid CrmContactId { get; set; }
        public string EmailAddress { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Telephone3 { get; set; }
        public string FullName
        {
            get
            {
                return (FirstName.Trim() + " " + Surname.Trim()).Trim();
            }
        }

        public bool IsActiveTenant { get; set; }
        public string HouseholdId { get; set; }
        public string Accounttype { get; set; }
        public bool MainTenant { get; set; }
        }

}