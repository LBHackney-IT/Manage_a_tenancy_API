using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class OfficerAreaPatch
    {
        public string officerId { get; set; }
        public string patchId { get; set; }
        public string areamanagerId { get; set; }
        public bool isUpdatingPatch { get; set; }
        public string updatedByOfficer { get; set; }
        public bool deleteExistingRelationship { get; set; }


    }
}
