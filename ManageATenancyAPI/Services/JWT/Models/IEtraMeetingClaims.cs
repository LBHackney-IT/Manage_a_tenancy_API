using System;

namespace ManageATenancyAPI.Services.JWT.Models
{
    public interface IEtraMeetingClaims
    {

        Guid MeetingId { get; set; }
    }
}