using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageATenancyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ManageATenancyAPI
{
    public partial class UhwDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public UhwDbContext(DbContextOptions<UhwDbContext> options) : base(options)
        {
        }

    }
}