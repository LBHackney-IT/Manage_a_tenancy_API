using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class AreaPatch
    {
        public string HackneyAreaId { get; set; }
        public string HackneyareaName { get; set; }
        public string HackneyPropertyReference { get; set; }
        public string HackneyPostCode { get; set; }
        public string HackneyllpgReference { get; set; }
        public string HackneyManagerPropertyPatchId { get; set; }
        public string HackneyManagerPropertyPatchName { get; set; }
        public string HackneyWardId { get; set; }
        public string HackneyWardName { get; set; }
        public string HackneyEstateofficerPropertyPatchId { get; set; }
        public string HackneyEstateofficerPropertyPatchName { get; set; }
        public string HackneyEstateOfficerId { get; set; }
    }
}
