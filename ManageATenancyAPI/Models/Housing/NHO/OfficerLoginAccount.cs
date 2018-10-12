using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class OfficerLoginAccount
    {
        [JsonProperty("hackney_username")]
        public string HackneyUsername { get; set; }
        [JsonProperty("hackney_password")]
        public string HackneyPassword { get; set; }        
    }
}
