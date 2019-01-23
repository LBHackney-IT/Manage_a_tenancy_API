using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{
    public class TraRequest
    {
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Email { get; set; }
        public int AreaId { get; set; }
        public Guid PatchId { get; set; }
        public List<string> EstateRefs { get; set; }

    }
}
