using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManageATenancyAPI.Models
{
    public class Email
    {
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string EmailSubject { get; set; }
        public string EmailMessage { get; set; }
    }
}