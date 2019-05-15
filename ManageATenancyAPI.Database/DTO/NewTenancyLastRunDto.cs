using System;
using ManageATenancyAPI.Database.Models;

namespace ManageATenancyAPI.Database.DTO
{
    public class NewTenancyLastRunDto
    {
        public NewTenancyLastRunDto(NewTenancyLastRun model)
        {
            LastRun = model.LastRun;
        }
        
        public DateTime LastRun { get; set; }
    }
}