using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Interfaces.Housing
{
    public interface IHackneyHousingAPICallBuilder
    {
        Task<HttpClient> CreateRequest(string accessToken);
    }
}
