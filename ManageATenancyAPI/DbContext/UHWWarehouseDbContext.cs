using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ManageATenancyAPI.DbContext
{
    public partial class UHWWarehouseDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public UHWWarehouseDbContext(DbContextOptions<UHWWarehouseDbContext> options) : base(options)
        {
        }
    }
}
