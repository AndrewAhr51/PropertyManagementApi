using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments;
using System.Data;
using System.Security;

namespace PropertyManagementAPI.Infrastructure.Data
{

    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options) { }

        // Custom entities
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Emails> Emails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<RentInvoice> RentInvoices { get; set; }
        public DbSet<UtilityInvoice> UtilityInvoices { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Properties> Properties { get; set; }
        public DbSet<PropertyOwner> PropertyOwners { get; set; }
        public DbSet<PropertyTenant> PropertyTenants { get; set; }
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
        public DbSet<LkupCleaningType> LkupCleaningType { get; set; }
        public DbSet<SecurityDepositInvoice> SecurityDepositInvoices { get; set; }
        public DbSet<CleaningFeeInvoice> CleaningFeeInvoices { get; set; }
        public DbSet<LeaseTerminationInvoice> LeaseTerminationInvoices { get; set; }
        public DbSet<ParkingFeeInvoice> ParkingFeeInvoices { get; set; }
        public DbSet<PropertyTaxInvoice> PropertyTaxInvoices { get; set; }
        public DbSet<InsuranceInvoice> InsuranceInvoices { get; set; }
        public DbSet<LegalFeeInvoice> LegalFeeInvoices { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                Console.WriteLine($"Entity: {entity.Name}");
                foreach (var prop in entity.GetProperties())
                {
                    Console.WriteLine($"  Property: {prop.Name}, Type: {prop.ClrType}");
                }
            }

            // ✅ Enforce consistent PK mapping
            modelBuilder.Entity<Invoice>()
                .HasKey(x => x.InvoiceId);

            modelBuilder.Entity<Invoice>()
                .Property(x => x.InvoiceId)
                .HasColumnType("int");

            modelBuilder.Entity<LegalFeeInvoice>()
                .HasBaseType<Invoice>(); // Reinforce inheritance

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
