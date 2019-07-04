using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ManageATenancyAPI.Interfaces.Housing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Services.Housing
{
    public class HackneyHousingAPICall : IHackneyHousingAPICall
    {
        private string beginningOfQuery;
        
        public async Task<HttpResponseMessage> getHousingAPIResponse(HttpClient httpClient, string query,string parameter)
        {
            var response = new HttpResponseMessage();
            try
            {
                response = httpClient.GetAsync(query).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new ServiceException();
                }
            }
            catch (Exception ex)
            {
            
                response.StatusCode = HttpStatusCode.BadRequest;
                throw new ServiceException();
            }
            return response;
        }

        public async Task<HttpResponseMessage> SendAsJsonAsync<T>(HttpClient client, HttpMethod method, string requestUri, T value)
        {
            var response = new HttpResponseMessage();
            try
            {
                var content = value.GetType().Name.Equals("JObject") ?
                value.ToString() :
                JsonConvert.SerializeObject(value, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });

                HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = new StringContent(content) };
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                response = await client.SendAsync(request).ConfigureAwait(false);
           
                if (!response.IsSuccessStatusCode)
                {
                    throw new ServiceException();
                }
            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.BadRequest;
                throw new ServiceException();
            }
            return response;
        }

        public async Task<HttpResponseMessage> postHousingAPI(HttpClient client, string query, JObject jObject)
        {
            var response = new HttpResponseMessage();
            //try
            //{
                var content = new StringContent(jObject.ToString(), Encoding.UTF8, "application/json");
                response = await client.PostAsync(query, content).ConfigureAwait(false) ;
               
                if (!response.IsSuccessStatusCode)
                {
                    throw new ServiceException();
                }
            //}
            //catch (Exception ex)
            //{
                
            //    response.StatusCode = HttpStatusCode.BadRequest;
            //    throw new ServiceException();
            //}
            return response;

        }

        public async Task<bool> UpdateObject(HttpClient client, string requestUri, JObject updateObject)
        {
            HttpResponseMessage updateResponse;
            var method = new HttpMethod("PATCH");
            string jsonString = JsonConvert.SerializeObject(updateObject);
            HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json") };
            try
            {
                updateResponse = await client.SendAsync(request);
                if (updateResponse.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<HttpResponseMessage> deleteObjectAPIResponse(HttpClient client, string query)
        {
            var response = new HttpResponseMessage();
            try
            {
                response = client.DeleteAsync(query).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new ServiceException();
                }
            }
            catch (Exception ex)
            {

                response.StatusCode = HttpStatusCode.BadRequest;
                throw new ServiceException();
            }
            return response;
        }

     }

    public class ServiceException : System.Exception
    {

    }

}

