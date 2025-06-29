using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Documents;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Maintenance;
using PropertyManagementAPI.Domain.Entities.Notes;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Properties;
using PropertyManagementAPI.Domain.Entities.Roles;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Domain.Entities.Vendors;

namespace PropertyManagementAPI.Infrastructure.Data
{
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Emails> Emails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentStorage> DocumentStorage { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<PreferredMethod> PreferredMethods { get; set; }
        public DbSet<Pricing> Pricing { get; set; }
        public DbSet<Propertys> Properties { get; set; }
        public DbSet<PropertyOwner> PropertyOwners { get; set; }
        public DbSet<PropertyTenant> PropertyTenants { get; set; }
        public DbSet<PropertyPhotos> PropertyPhotos { get; set; }
        public DbSet<MaintenanceRequests> MaintenanceRequests { get; set; }
        public DbSet<LkupInvoiceType> LkupInvoiceType { get; set; }
        public DbSet<LkupUtilities> LkupUtilities { get; set; }
        public DbSet<LkupCleaningType> LkupCleaningType { get; set; }
        public DbSet<PaymentAuditLog> PaymentAuditLogs { get; set; } = null!;
        public DbSet<CleaningFeeInvoice> CleaningFeeInvoices { get; set; } = null!;
        public DbSet<InsuranceInvoice> InsuranceInvoices { get; set; } = null!;
        public DbSet<RentInvoice> RentInvoices { get; set; } = null!;
        public DbSet<UtilityInvoice> UtilityInvoices { get; set; } = null!;
        public DbSet<SecurityDepositInvoice> SecurityDepositInvoices { get; set; } = null!;
        public DbSet<PropertyTaxInvoice> PropertyTaxInvoices { get; set; } = null!;
        public DbSet<LeaseTerminationInvoice> LeaseTerminationInvoices { get; set; } = null!;
        public DbSet<LegalFeeInvoice> LegalFeeInvoices { get; set; } = null!;
        public DbSet<CardToken> CardTokens { get; set; } = null!;
        public DbSet<BankAccountInfo> BankAccountInfo { get; set; } = null!;
        public DbSet<ParkingFeeInvoice> ParkingFeeInvoices { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Invoice TPT Inheritance
            modelBuilder.Entity<Invoice>().ToTable("Invoices");
            modelBuilder.Entity<RentInvoice>().ToTable("RentInvoices").HasBaseType<Invoice>();
            modelBuilder.Entity<UtilityInvoice>().ToTable("UtilityInvoices").HasBaseType<Invoice>();
            modelBuilder.Entity<SecurityDepositInvoice>().ToTable("SecurityDepositInvoices").HasBaseType<Invoice>();
            modelBuilder.Entity<CleaningFeeInvoice>().ToTable("CleaningFeeInvoices").HasBaseType<Invoice>();
            modelBuilder.Entity<LeaseTerminationInvoice>().ToTable("LeaseTerminationInvoices").HasBaseType<Invoice>();
            modelBuilder.Entity<ParkingFeeInvoice>().ToTable("ParkingFeeInvoices").HasBaseType<Invoice>();
            modelBuilder.Entity<PropertyTaxInvoice>().ToTable("PropertyTaxInvoices").HasBaseType<Invoice>();
            modelBuilder.Entity<InsuranceInvoice>().ToTable("InsuranceInvoices").HasBaseType<Invoice>();
            modelBuilder.Entity<LegalFeeInvoice>().ToTable("LegalFeeInvoices").HasBaseType<Invoice>();

            // Payment TPT Inheritance
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<CardPayment>().ToTable("CardPayments").HasBaseType<Payment>();
            modelBuilder.Entity<CheckPayment>().ToTable("CheckPayments").HasBaseType<Payment>();
            modelBuilder.Entity<ElectronicTransferPayment>().ToTable("ElectronicTransferPayments").HasBaseType<Payment>();
            modelBuilder.Entity<WireTransfer>().ToTable("WireTransfers").HasBaseType<Payment>();
            modelBuilder.Entity<CreditCardPayment>().ToTable("CreditCardPayments").HasBaseType<Payment>();

            // Payment Relationships
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Tenant)
                .WithMany(t => t.Payments)
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Owner)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // PreferredMethod Relationships
            modelBuilder.Entity<PreferredMethod>()
                .HasOne(pm => pm.CardToken)
                .WithMany()
                .HasForeignKey(pm => pm.CardTokenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PreferredMethod>()
                .HasOne(pm => pm.BankAccountInfo)
                .WithMany(b => b.PreferredMethods)
                .HasForeignKey(pm => pm.BankAccountInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PreferredMethod>()
                .HasOne(pm => pm.Tenant)
                .WithMany()
                .HasForeignKey(pm => pm.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PreferredMethod>()
                .HasOne(pm => pm.Owner)
                .WithMany()
                .HasForeignKey(pm => pm.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PaymentAuditLog
            modelBuilder.Entity<PaymentAuditLog>(entity =>
            {
                entity.ToTable("PaymentAuditLog");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Gateway).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Action).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
                entity.Property(e => e.PerformedBy).HasMaxLength(100);
                entity.Property(e => e.ResponsePayload).HasColumnType("json");

                entity.HasOne(e => e.Payment)
                      .WithMany()
                      .HasForeignKey(e => e.PaymentId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}