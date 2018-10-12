using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class Contact
    {
            public Guid CrMcontactId { get; set; }
            public string Title { get; set; }
            public DateTime? DateOfBirth { get; set; }
             public string LastName { get; set; }
            public string FirstName { get; set; }
            public string Email { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
            public string Telephone1 { get; set; }
            public string Telephone2 { get; set; }
            public string Telephone3 { get; set; }
            public string LARN { get; set; }
            public string HousingId { get; set; }
            public string USN { get; set; }
            public string CreatedByOfficer { get; set; }
            public string UpdatedByOfficer { get; set; }
            public string UPRN { get; set; }
            public string FullAddressSearch { get; set; }
            public string FullAddressDisplay { get; set; }

        public string FullName
            {
                get
                {
                    string strLN = string.Empty;
                    string strFN = string.Empty;
                    if (LastName != null)
                    {
                        strLN = LastName;
                    }
                    if (FirstName != null)
                    {
                        strFN = FirstName;
                    }
                    return (strFN.Trim() + " " + strLN.Trim()).Trim();
                }
            }
        
    }
}
