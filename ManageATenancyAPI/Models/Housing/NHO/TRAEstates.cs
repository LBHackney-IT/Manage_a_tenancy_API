using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace ManageATenancyAPI.Models.Housing.NHO
{

    [Table("TraEstates")]
    public class TraEstate
    {
        public int Id { get; set; }
        public int TraId { get; set; }
        public string EstateUHRef { get; set; }

        public string EstateName { get; set; }
    }
}
