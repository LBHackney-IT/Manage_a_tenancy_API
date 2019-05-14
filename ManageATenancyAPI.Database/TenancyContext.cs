using ManageATenancyAPI.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ManageATenancyAPI.Database
{
    public class TenancyContextFactory : IDesignTimeDbContextFactory<TenancyContext>
    {
        public TenancyContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TenancyContext>();
            builder.UseSqlServer("Server=localhost;Database=ManageATenancy;Trusted_Connection=True;MultipleActiveResultSets=true");
            return new TenancyContext(builder.Options);
        }
    }
    
    public class TenancyContext : DbContext
    {
        public TenancyContext(DbContextOptions options)
            : base(options)
        { }
        
        public DbSet<NewTenancyLastRun> NewTenancyLastRun { get; set; }
    }
}