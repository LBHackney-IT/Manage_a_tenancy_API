using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class EstateOfficerAccount
    {
        [JsonProperty("officeraccount")]
        public OfficerAccount OfficerAccount  { get; set; }
        [JsonProperty("officerloginaccount")]
        public OfficerLoginAccount OfficerLoginAccount{ get; set; }
    }
}
