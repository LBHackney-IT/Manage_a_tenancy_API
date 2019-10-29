using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ManageATenancyAPI.UseCases.Meeting.SaveMeeting.Boundary
{
    public class SignOff
    {
        /// <summary>
        /// Base 64 encoded signature image
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// Name of TRA rep
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// TRA role
        /// </summary>
        [Required]
        public string Role { get; set; }

        [JsonIgnore]
        public Guid SignatureId { get; set; }

        public DateTime SignOffDate { get; set; }
    }
}