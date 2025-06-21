using System;
using System.Diagnostics;

using Base.Entities;
using Base.Helper;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Base.Persistence
{
    public class BaseApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public BaseApplicationDbContext(DbContextOptions<BaseApplicationDbContext> options): base(options)
        {
            
        }
        public BaseApplicationDbContext() : base()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //string dbFileName = configuration["ConnectionStrings:DbFileName"];
                //optionsBuilder.UseSqlite($"Data Source={dbFileName}");
                ////optionsBuilder.UseLoggerFactory(GetLoggerFactory());

                string connectionString = ConfigurationHelper.GetConfiguration("DefaultConnection", "ConnectionStrings");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }



    }
}
