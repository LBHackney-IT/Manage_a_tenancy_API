using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageATenancyAPI.Models
{
    public class TRA
    {
        public int TRAId { get; set; }
        public string Name { get; set; }
        public int AreaId { get; set; }
        public string TRAEmail { get; set; }

    }

    public class TRAInformation
    {
        public TRA TRA{ get; set; }
        public string PatchId { get; set; }
        public List<TRAEstate> ListOfEstates { get; set; }
        public List<TRARolesAssignment> ListOfRoles { get; set; }

    }

    public class TRAEstate
    {
        public int TRAId { get; set; }
        public string EstateUHReference { get; set; }
        public string EstateName { get; set; }
    }

    public class TRARolesAssignment
    {
        public int TRAId { get; set; }
        public int? RoleId { get; set; }
        public string RoleName { get; set; }      
        public string PersonName { get; set; }
    }
}
