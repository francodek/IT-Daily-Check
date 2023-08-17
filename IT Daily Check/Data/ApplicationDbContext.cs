using IT_Daily_Check.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IT_Daily_Check.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<DailyCheck> DailyChecks { get; set; }
        public DbSet<CCTV> CCTVs { get; set; }
        public DbSet<CCTVcheck> CCTVchecks { get; set; }
        public DbSet<DeviceService> DeviceServices { get; set; }
        public DbSet<DeviceServicecheck> DeviceServicechecks { get; set; }
        public DbSet<InternetServiceProvider> InternetServiceProviders { get; set; }
        public DbSet<InternetServiceSpeedcheck> InternetServiceSpeedchecks { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Result> Results { get; set; }

    }
}
//database context
