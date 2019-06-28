using System;

namespace ManageATenancyAPI.Services.JWT.Models
{
    public interface IMeetingClaims
    {

        Guid MeetingId { get; set; }
    }
}