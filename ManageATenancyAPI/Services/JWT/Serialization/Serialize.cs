using ManageATenancyAPI.Services.JWT.Models;
using Newtonsoft.Json;

namespace ManageATenancyAPI.Services.JWT.Serialization
{
    public static class Serialize
    {
        public static string ToJson(this ManageATenancyClaims self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }
}