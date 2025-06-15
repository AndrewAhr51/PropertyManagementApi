using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities;
using System.Data;
using System.Security;

namespace PropertyManagementAPI.Infrastructure.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        // Custom entities
        public DbSet<User> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Emails> Emails { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Property> Property { get; set; }
        public DbSet<PropertyOwner> PropertyOwners{ get; set; }
        public DbSet<PropertyPhotos> PropertyPhotos { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Pricing> Pricing { get; set; }
        public DbSet<MaintenanceRequests> MaintenanceRequests { get; set; }



    }
}
