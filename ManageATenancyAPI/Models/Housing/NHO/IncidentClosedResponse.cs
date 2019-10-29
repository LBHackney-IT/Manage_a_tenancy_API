using System;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class IncidentClosedResponse
    {
        public Guid IncidentId { get; set; }
        public string Status { get; set; }
    }
}