using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Interfaces
{
    public interface IHackneyGetCRM365Token
    {
        Task<string> getCRM365AccessToken();
        
    }
}
