using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Tests.Helpers
{
    public static class FakeHackneyHousingAPICallBuilder
    {
        public static HttpClient createFakeRequest(string response)
        {
            var _client = new HttpClient();
            _client.BaseAddress = new Uri("http://ValidOrganizationUrl");
            _client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            _client.DefaultRequestHeaders.Add("OData-Version", "4.0");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + response);

            return _client;
        }
    }
}
