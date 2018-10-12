using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class NextOfKin
    {
        public string nextOfKinName { get; set; }
        public string nextOfKinRelationship { get; set; }
        public string nextOfKinAddress { get; set; }
        public string nextOfKinMobile { get; set; }
        public string nextOfKinOtherTelehone { get; set; }
        public string nextOfKinEmail { get; set; }
        public Guid contactID { get; set; }
    }
}
