using System;
using System.ComponentModel.DataAnnotations;

namespace ManageATenancyAPI.Database.Models
{
    public class NewTenancyLastRun
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime LastRun { get; set; }
    }
}