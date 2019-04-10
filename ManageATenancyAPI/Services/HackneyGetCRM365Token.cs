using ManageATenancyAPI.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using MyPropertyAccountAPI.Configuration;

namespace ManageATenancyAPI.Services
{
    public class HackneyGetCRM365Token : IHackneyGetCRM365Token
    {
        private HttpClient httpClient;
        private readonly URLConfiguration configuration;
        private ILoggerAdapter<HackneyGetCRM365Token> logger;
        public HackneyGetCRM365Token(IOptions<URLConfiguration> config)
        {
            configuration = config?.Value;
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(configuration.ManageATenancyAPIURL);
        }

     
        public async Task<string> getCRM365AccessToken()
        {
            var response = new HttpResponseMessage();
            var token = "";
            try
            {
                response = httpClient.GetAsync("/GetCRM365AccessToken").Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new GetCRM365TokenServiceException();
                }
                var tokenJsonResponse = JsonConvert.DeserializeObject<JObject>(response.Content.ReadAsStringAsync().Result);
                token = tokenJsonResponse["result"].ToString();
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                throw new GetCRM365TokenServiceException();
            }
            return token;
        }

        public class GetCRM365TokenServiceException : System.Exception { }
  
       
    }
}
