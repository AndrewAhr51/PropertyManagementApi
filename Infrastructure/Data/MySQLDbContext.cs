using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Documents;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices.Base;
using PropertyManagementAPI.Domain.Entities.Maintenance;
using PropertyManagementAPI.Domain.Entities.Notes;
using PropertyManagementAPI.Domain.Entities.OwnerAnnouncements;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.Banking;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Domain.Entities.Payments.PreferredMethods;
using PropertyManagementAPI.Domain.Entities.Payments.Quickbooks;
using PropertyManagementAPI.Domain.Entities.Property;
using PropertyManagementAPI.Domain.Entities.Roles;
using PropertyManagementAPI.Domain.Entities.TenantAnnouncements;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Domain.Entities.Vendors;
using PropertyManagementAPI.Infrastructure.Data.Configurations.Documents;
using BankAccount = PropertyManagementAPI.Domain.Entities.Payments.Banking.BankAccount;
using Invoice = PropertyManagementAPI.Domain.Entities.Invoices.Invoice;
using InvoiceLineItem = PropertyManagementAPI.Domain.Entities.Invoices.InvoiceLineItem;

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
        public DbSet<InvoiceDocuments> InvoiceDocuments { get; set; }
        public DbSet<InvoiceType> InvoiceTypes { get; set; }
        public DbSet<InvoiceLineItem> InvoiceLineItems { get; set; }
        public DbSet<InvoiceLineItemMetadata> InvoiceLineItemMetadata { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Lease> Leases { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentReference> DocumentReferences { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<PreferredMethod> PreferredMethods { get; set; }
        public DbSet<Pricing> Pricing { get; set; }
        public DbSet<Properties> Properties { get; set; }
        public DbSet<PropertyOwner> PropertyOwners { get; set; }
        public DbSet<PropertyTenant> PropertyTenants { get; set; }
        public DbSet<PropertyPhotos> PropertyPhotos { get; set; }
        public DbSet<MaintenanceRequests> MaintenanceRequests { get; set; }
        public DbSet<lkupUtilities> LkupUtilities { get; set; }
        public DbSet<lkupCleaningType> LkupCleaningType { get; set; }
        public DbSet<PaymentAuditLog> PaymentAuditLogs { get; set; } = null!;
        public DbSet<CardToken> CardTokens { get; set; } = null!;
        public DbSet<BankAccount> BankAccount { get; set; } = null!;
        public DbSet<CardPayment> CardPayments { get; set; } = null!;
        public DbSet<TenantAnnouncement> TenantAnnouncements { get; set; } = null!;
        public DbSet<OwnerAnnouncement> OwnerAnnouncements { get; set; } = null!;
        public DbSet<ACHAuthorization> ACHAuthorizations { get; set; } = null!;
        public DbSet<PaymentTransactions> PaymentTransactions { get; set; } = null!;
        public DbSet<ElectronicTransferPayment> ElectronicTransferPayments { get; set; } = null!;
        public DbSet<WireTransfer> WireTransfers { get; set; } = null!;
        public DbSet<CheckPayment> CheckPayments { get; set; } = null!;
        public DbSet<PaymentMetadata> PaymentMetadata { get; set; } = null!;
        public DbSet<StripeWebhookEvent> StripeWebhookEvents { get; set; }
        public DbSet<QuickBooksAuditLog> QuickBooksAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Payment TPT Inheritance
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<CardPayment>().ToTable("CardPayments").HasBaseType<Payment>();
            modelBuilder.Entity<CheckPayment>().ToTable("CheckPayments").HasBaseType<Payment>();
            modelBuilder.Entity<ElectronicTransferPayment>().ToTable("ElectronicTransferPayments").HasBaseType<Payment>();
            modelBuilder.Entity<WireTransfer>().ToTable("WireTransfers").HasBaseType<Payment>();

            // PreferredMethod Relationships
            modelBuilder.Entity<PreferredMethod>()
                .HasOne(pm => pm.CardToken)
                .WithMany()
                .HasForeignKey(pm => pm.CardTokenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PreferredMethod>()
                .HasOne(pm => pm.BankAccount)
                .WithMany(b => b.PreferredMethods)
                .HasForeignKey(pm => pm.BankAccountId)
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

            modelBuilder.Entity<Invoice>().ToTable("invoices");
            modelBuilder.Entity<InvoiceDocuments>().ToTable("InvoiceDocuments");
            modelBuilder.Entity<InvoiceLineItemMetadata>()
                .HasKey(m => new { m.LineItemId, m.MetaKey });

            modelBuilder.Entity<InvoiceLineItemMetadata>()
                .HasOne(m => m.LineItem)
                .WithMany(li => li.Metadata)
                .HasForeignKey(m => m.LineItemId);

            modelBuilder.Entity<InvoiceLineItem>(entity =>
            {
                entity.ToTable("InvoiceLineItems");

                entity.HasKey(e => e.LineItemId);

                entity.HasOne(e => e.Invoice)
                    .WithMany(i => i.LineItems)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.InvoiceType)
                    .WithMany(t => t.LineItems)
                    .HasForeignKey(e => e.LineItemTypeId) // ✅ Correct FK column name
                    .OnDelete(DeleteBehavior.Restrict);    // Or Cascade, based on business rules

                entity.HasMany(e => e.Metadata)
                    .WithOne(m => m.LineItem)
                    .HasForeignKey(m => m.LineItemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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

            modelBuilder.Entity<InvoiceDocuments>().ToTable("InvoiceDocuments"); // Base entity
            modelBuilder.Entity<Invoice>().ToTable("Invoices"); // Derived TPT entity

            modelBuilder.Entity<QuickBooksAuditLog>()
                .Property(x => x.EventType).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<QuickBooksAuditLog>()
                .Property(x => x.RealmId).IsRequired().HasMaxLength(64);

            // Document & Reference Configuration
            modelBuilder.ApplyConfiguration(new DocumentConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentReferenceConfiguration());
        }
    }
}