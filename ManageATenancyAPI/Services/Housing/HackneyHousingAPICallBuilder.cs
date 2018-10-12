using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces;
using ManageATenancyAPI.Interfaces.Housing;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Services.Housing
{
    public class HackneyHousingAPICallBuilder : IHackneyHousingAPICallBuilder
    {
    
        private readonly ILoggerAdapter<HackneyHousingAPICallBuilder> _logger;
        private  HttpClient _client;
        private readonly URLConfiguration _configuration;

        public HackneyHousingAPICallBuilder(IOptions<URLConfiguration> config, ILoggerAdapter<HackneyHousingAPICallBuilder> logger)
        {
            _logger = logger;
           
            _configuration = config?.Value;
        }
        public async Task<HttpClient> CreateRequest(string accessToken)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_configuration.CRM365OrganizationUrl);
            _client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            _client.DefaultRequestHeaders.Add("OData-Version", "4.0");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            return _client;
        }
    }
}
