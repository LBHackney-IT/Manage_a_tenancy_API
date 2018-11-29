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
        [Key]
        public int TRAId { get; set; }

        [Key]
        public string EstateUHRef { get; set; }

        public string EstateName { get; set; }
    }
}
