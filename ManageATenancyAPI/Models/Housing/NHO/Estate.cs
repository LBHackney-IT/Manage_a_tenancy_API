using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models.UniversalHousing;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class Estate
    {
        public string EstateId { get; set; }
        public string EstateName { get; set; }
        public IEnumerable<Block> Blocks { get; set; }

        public static Estate FromModel(UhProperty property)
        {
            var estate = new Estate();
            estate.EstateId = property.prop_ref.Trim();
            estate.EstateName = property.short_address.Trim();
            return estate;
        }
    }
}
