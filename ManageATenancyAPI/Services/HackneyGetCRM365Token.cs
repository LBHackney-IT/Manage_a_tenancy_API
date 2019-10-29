using ManageATenancyAPI.Interfaces;
using Microsoft.Extensions.Options;
using MyPropertyAccountAPI.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Services
{
    public class HackneyGetCRM365Token : IHackneyGetCRM365Token
    {
        private readonly HttpClient httpClient;
        private readonly URLConfiguration configuration;
        public HackneyGetCRM365Token(IOptions<URLConfiguration> config)
        {
            configuration = config?.Value;
            httpClient = new HttpClient();
        }


        public async Task<string> getCRM365AccessToken()
        {
            var response = new HttpResponseMessage();
            try
            {
                response = await httpClient.GetAsync($"{configuration.HackneyAPIUrl}/GetCRM365AccessToken");
                if (!response.IsSuccessStatusCode)
                    throw new GetCRM365TokenServiceException();

                var tokenJsonResponse = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                var token = tokenJsonResponse["result"].ToString();
                  return token;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                throw new GetCRM365TokenServiceException();
            }
        }

        public class GetCRM365TokenServiceException : System.Exception { }


    }
}