using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.UniversalHousing;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class Block
    {
        public string BlockName { get; set; }
        public string BlockId { get; set; }
        public string EstateId { get; set; }
        public string EstateName { get; set; }
        public static Block FromModel(UhProperty property)
        {
            var eb = new Block
            {
                EstateId = property.major_ref.Trim(),
                BlockId = property.prop_ref.Trim(),
                BlockName = property.short_address.Trim()
            };

            return eb;
        }
    }
}
