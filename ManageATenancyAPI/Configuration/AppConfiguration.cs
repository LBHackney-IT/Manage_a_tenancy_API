using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Configuration
{
    public class AppConfiguration
    {
        public string CompletedClosureType { get; set; }
        public string MaxCISearchResults { get; set; }

        public string EncryptionKey { get; set; }
        public string ETRAAccount { get; set; }
        public bool EnableAdmin { get; set; }
    }
}
