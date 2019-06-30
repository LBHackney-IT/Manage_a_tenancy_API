using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Services.JWT.Models
{
   public interface IMeetingClaims
    {
        Guid MeetingId { get; set; }
    }
}
