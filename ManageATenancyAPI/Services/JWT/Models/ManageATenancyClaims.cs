using System;
using ManageATenancyAPI.Services.JWT.Serialization;
using Newtonsoft.Json;

namespace ManageATenancyAPI.Services.JWT.Models
{
    public partial class ManageATenancyClaims
    {
        public static ManageATenancyClaims FromJson(string json) => JsonConvert.DeserializeObject<ManageATenancyClaims>(json, Converter.Settings);
    }

    public partial class ManageATenancyClaims: IManageATenancyClaims
    {
        [JsonProperty("estateOfficerLoginId")]
        public Guid EstateOfficerLoginId { get; set; }

        [JsonProperty("officerId")]
        public Guid OfficerId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("areaManagerId")]
        public Guid AreaManagerId { get; set; }

        [JsonProperty("officerPatchId")]
        public Guid OfficerPatchId { get; set; }

        [JsonProperty("areaId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public int AreaId { get; set; }
    }
}