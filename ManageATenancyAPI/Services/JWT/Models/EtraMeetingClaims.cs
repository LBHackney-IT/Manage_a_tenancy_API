using System;
using ManageATenancyAPI.Services.JWT.Serialization;
using Newtonsoft.Json;

namespace ManageATenancyAPI.Services.JWT.Models
{
    public partial class EtraMeetingClaims : IEtraMeetingClaims
    {
        [JsonProperty("meetingId")]
        public Guid MeetingId { get; set; }

        public static EtraMeetingClaims FromJson(string json) => JsonConvert.DeserializeObject<EtraMeetingClaims>(json, Converter.Settings);
    }
}