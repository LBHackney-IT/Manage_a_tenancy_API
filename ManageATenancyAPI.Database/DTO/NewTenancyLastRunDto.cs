using System;
using ManageATenancyAPI.Database.Entities;

namespace ManageATenancyAPI.Database.DTO
{
    public class NewTenancyLastRunDto
    {
        public NewTenancyLastRunDto(DateTime lastRun)
        {
            LastRun = lastRun;
        }
        
        public NewTenancyLastRunDto(NewTenancyLastRun model)
        {
            LastRun = model.LastRun;
        }
        
        public DateTime LastRun { get; set; }
    }
}