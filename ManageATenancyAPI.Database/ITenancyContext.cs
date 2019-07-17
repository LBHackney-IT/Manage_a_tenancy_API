using ManageATenancyAPI.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManageATenancyAPI.Database
{
    public interface ITenancyContext
    {
        DbSet<NewTenancyLastRun> NewTenancyLastRun { get; set; }

        void SaveChanges();
    }
}