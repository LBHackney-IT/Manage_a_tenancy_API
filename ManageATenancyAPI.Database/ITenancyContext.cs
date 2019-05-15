using ManageATenancyAPI.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ManageATenancyAPI.Database
{
    public interface ITenancyContext
    {
        DbSet<NewTenancyLastRun> NewTenancyLastRun { get; set; }
    }
}