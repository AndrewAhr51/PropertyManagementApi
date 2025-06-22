using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Domain.Entities.Invoices;
using System.Data;
using System.Security;

namespace PropertyManagementAPI.Infrastructure.Data
{

    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options) { }

        // Custom entities
        public DbSet<User> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Emails> Emails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<RentInvoice> RentInvoices { get; set; }
        public DbSet<UtilityInvoice> UtilityInvoices { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Property> Property { get; set; }
        public DbSet<PropertyOwner> PropertyOwners{ get; set; }
        public DbSet<PropertyPhotos> PropertyPhotos { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Pricing> Pricing { get; set; }
        public DbSet<MaintenanceRequests> MaintenanceRequests { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentStorage> DocumentStorage { get; set; }
        public DbSet<CreditCardInfo> CreditCardInfo { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<LkupInvoiceType> LkupInvoiceType { get; set; }
        public DbSet<LkupUtilities> LkupUtilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TPT inheritance
            modelBuilder.Entity<Invoice>().ToTable("Invoices");
            modelBuilder.Entity<RentInvoice>().ToTable("RentInvoices");
            modelBuilder.Entity<UtilityInvoice>().ToTable("UtilityInvoices");
            modelBuilder.Entity<SecurityDepositInvoice>().ToTable("SecurityDepositInvoices");
            modelBuilder.Entity<CleaningFeeInvoice>().ToTable("CleaningFeeInvoices");
            modelBuilder.Entity<LeaseTerminationInvoice>().ToTable("LeaseTerminationInvoices");
            modelBuilder.Entity<ParkingFeeInvoice>().ToTable("ParkingFeeInvoices");
            modelBuilder.Entity<PropertyTaxInvoice>().ToTable("PropertyTaxInvoices");
            modelBuilder.Entity<InsuranceInvoice>().ToTable("InsuranceInvoices");
            modelBuilder.Entity<LegalFeeInvoice>().ToTable("LegalFeeInvoices");

            // Optional: additional model configuration goes here
        }

    }
}
