using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities;
using System.Data;

namespace PropertyManagementAPI.Infrastructure.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        // Custom entities
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Emails> Emails { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
    }
}
