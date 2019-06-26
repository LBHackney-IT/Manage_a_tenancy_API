using System.Collections.Generic;
using ManageATenancyAPI.Database.Models;

namespace ManageATenancyAPI.Controllers.v2
{
    public class GetAllTRAsOutputModel
    {
        public IList<TRA> TRAs { get; set; }
    }
}