using DAC.core.models;
using DAC.kernel.models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DynamicAppCreator.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<SqlServers> SqlServers { get; set; }

        public DbSet<SqlDatabases> SqlDatabases { get; set; }

        public DbSet<SqlTables> SqlTables { get; set; }

        public DbSet<PermissionGroups> PermissionGroups { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<PermissionSubSettings> PermissionSubSettings { get; set; }
        public DbSet<AppLogger> AppLogger { get; set; }
        public DbSet<Modules> Modules { get; set; }
    }


}
