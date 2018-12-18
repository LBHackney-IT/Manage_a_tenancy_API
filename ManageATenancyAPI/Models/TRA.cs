using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace ManageATenancyAPI.Models
{

    [Table("TRA")]
    public class TRA
    {

        [Key]
        public int TRAId { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Email { get; set; }
        public int AreaId { get; set; }
        public Guid PatchId { get; set; }
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
        public string Role { get; set; }
        public string RoleName { get; set; }      
        public string PersonName { get; set; }
    }
}
