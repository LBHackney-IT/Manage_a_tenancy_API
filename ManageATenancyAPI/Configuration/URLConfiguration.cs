using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPropertyAccountAPI.Configuration
{
    public class URLConfiguration
    {
        public string CRM365OrganizationUrl { get; set; }
        public string ManageATenancyAPIURL { get; set; }
        public string BankHolidaysUrl { get; set; }
        public string HackneyAPIUrl { get; set; }
        public string HackneyAPIkey { get; set; }
    }
}
