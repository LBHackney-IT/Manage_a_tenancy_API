using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace ManageATenancyAPI.Interfaces.Housing
{
    public interface IHackneyHousingAPICall
    {
        Task<HttpResponseMessage> getHousingAPIResponse(HttpClient httpClient, string query, string parameter);
        Task<HttpResponseMessage> SendAsJsonAsync<T>(HttpClient client, HttpMethod method, string requestUri, T value);
        Task<HttpResponseMessage> postHousingAPI(HttpClient client, string query, JObject contact);
        Task<bool> UpdateObject(HttpClient client, string requestUri, JObject updateObject);        
        Task<HttpResponseMessage> deleteObjectAPIResponse(HttpClient client, string query);

     }
}
