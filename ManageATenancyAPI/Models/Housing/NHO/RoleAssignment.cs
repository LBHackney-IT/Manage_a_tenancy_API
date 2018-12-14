using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models.Housing.NHO
{

    public class RoleAssignment
    {

        public int TraId { get; set; }
        public Guid ContactId { get; set; }
        public string Role { get; set; }
        public string PersonName { get; set; }
    }
}
