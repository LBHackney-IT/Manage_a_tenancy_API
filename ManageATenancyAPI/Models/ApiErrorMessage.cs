using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models
{
    public class ApiErrorMessage
    {
        public string developerMessage { get; set; }
        public string userMessage { get; set; }
        public string source { get; set; }
        public string stackTrace { get; set; }
    }
}
