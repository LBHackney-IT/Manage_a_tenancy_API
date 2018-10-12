using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class OfficerAccount
    {
        [JsonProperty("hackney_firstname")]
        public string HackneyFirstname { get; set; }
        [JsonProperty("hackney_lastname")]
        public string HackneyLastname { get; set; }
        [JsonProperty("hackney_emailaddress")]
        public string HackneyEmailaddress { get; set; }
        [JsonProperty("hackney_name")]
        public string HackneyName
        {
            get
            {
                return (HackneyFirstname.Trim() + " " + HackneyLastname.Trim()).Trim();
            }

        }
    }
}
