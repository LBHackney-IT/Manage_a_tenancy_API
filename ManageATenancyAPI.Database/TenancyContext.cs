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
            builder.UseSqlServer("Server=localhost,1433;Database=ManageATenancy;MultipleActiveResultSets=true;User ID=SA;Password=Rooty-Tooty");
            return new TenancyContext(builder.Options);
        }
    }
    
    public class TenancyContext : DbContext, ITenancyContext
    {
        public TenancyContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<HousingArea> HousingAreas { get; set; }
        public DbSet<HousingAreaPatch> HousingAreaPatches { get; set; }

        public DbSet<NewTenancyLastRun> NewTenancyLastRun { get; set; }
        public DbSet<TRA> TRAs { get; set; }
        


        public new void SaveChanges()
        {
            base.SaveChanges();
        }
    }
}