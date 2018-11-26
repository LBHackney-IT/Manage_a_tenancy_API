using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class AddEstateToTraRequest
    {
        public int TraId { get; set; }
        public string EstateId { get; set; }
    }
}
