﻿using System;
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
}
