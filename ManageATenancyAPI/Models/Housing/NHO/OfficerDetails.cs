using System;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class OfficerDetails
    {
        public string Id { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public DateTime? LastNewTenancyCheck { get; set; }

        private OfficerDetails() { }

        public static OfficerDetails Create(dynamic officerDetails)
        {
            return new OfficerDetails
            {
                Id = officerDetails["hackney_estateofficerid"],
                EmailAddress = officerDetails["hackney_emailaddress"],
                FullName = officerDetails["hackney_name"],
                Forename = officerDetails["hackney_firstname"],
                Surname = officerDetails["hackney_lastname"],
                LastNewTenancyCheck = officerDetails["hackney_lastnewtenancycheckdate"]
            };
        }
    }
}