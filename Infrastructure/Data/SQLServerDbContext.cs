﻿using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Documents;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Maintenance;
using PropertyManagementAPI.Domain.Entities.Notes;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Properties;
using PropertyManagementAPI.Domain.Entities.Roles;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Domain.Entities.Vendors;
using System.Data;
using System.Security;

namespace PropertyManagementAPI.Infrastructure.Data
{

    public class SQLServerDbContext : DbContext
    {
        public SQLServerDbContext(DbContextOptions<SQLServerDbContext> options) : base(options) { }

        // Custom entities
        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Emails> Emails { get; set; }
        //public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Propertys> Property { get; set; }
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
    }
}
