using System;

namespace ManageATenancyAPI.Services.JWT.Models
{
    public class  MeetingClaims : IMeetingClaims
    {
       public Guid MeetingId { get ; set ; }
       public string OfficerName { get; set; }
       public int TraId { get; set; }
    }
}
