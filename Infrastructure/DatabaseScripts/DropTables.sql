USE PropertyManagement;
-- 🔧 Disable FK checks to prevent dependency issues
SET FOREIGN_KEY_CHECKS = 0;

-- 📄 Drop tenant/invoice adjustment triggers
DROP TRIGGER IF EXISTS trg_after_invoice_insert;
DROP TRIGGER IF EXISTS trg_after_invoice_update;
DROP TRIGGER IF EXISTS trg_before_invoice_delete;

-- 📑 Drop audit triggers for invoices
DROP TRIGGER IF EXISTS trg_audit_invoicedocuments_insert;
DROP TRIGGER IF EXISTS trg_audit_invoicedocuments_update;
DROP TRIGGER IF EXISTS trg_audit_invoicedocuments_delete;

-- 📦 Drop billing address audit triggers
DROP TRIGGER IF EXISTS LogBillingAddressInsert;
DROP TRIGGER IF EXISTS LogBillingAddressUpdate;
DROP TRIGGER IF EXISTS LogBillingAddressDelete;

-- 🔧 Re-enable FK checks
SET FOREIGN_KEY_CHECKS = 1;

-- ✅ Drop tables with foreign key dependencies first
DROP TABLE IF EXISTS QuickBooksAuditLog;
DROP TABLE IF EXISTS InvoiceAuditLog;
DROP TABLE IF EXISTS RolePermissions;
DROP TABLE IF EXISTS PaymentReminders;
DROP TABLE IF EXISTS DocumentReferences;
DROP TABLE IF EXISTS Documents;
DROP TABLE IF EXISTS Notes;
DROP TABLE IF EXISTS triggerlog;
DROP TABLE IF EXISTS PaymentAuditLog;
DROP TABLE IF EXISTS PaymentTransactions;

-- ✅ Drop invoice subtype tables before Invoices


DROP TABLE IF EXISTS InvoiceTypeMappings;
DROP TABLE IF EXISTS InvoiceLineItemMetadata;
DROP TABLE IF EXISTS InvoiceLineItems;
DROP TABLE IF EXISTS InvoiceType;

DROP TABLE IF EXISTS PaymentMetadata;
DROP TABLE IF EXISTS ElectronicTransferPayments;
DROP TABLE IF EXISTS TenantBankAccount;
DROP TABLE IF EXISTS OwnerBankAccount; 
DROP TABLE IF EXISTS PreferredMethod;
DROP TABLE IF EXISTS ACHAuthorizations;
DROP TABLE IF EXISTS BankAccounts;
DROP TABLE IF EXISTS CardToken; 
DROP TABLE IF EXISTS SpecialInstructions;
DROP TABLE IF EXISTS CheckPayments;
DROP TABLE IF EXISTS WireTransfers;
DROP TABLE IF EXISTS CardPayments;

DROP TABLE IF EXISTS Payments;

-- ✅ Drop base invoice table
DROP TABLE IF EXISTS Invoices;
DROP TABLE IF EXISTS InvoiceDocuments;


-- ✅ Continue with remaining dependent tables
DROP TABLE IF EXISTS Leases;
DROP TABLE IF EXISTS PropertyPhotos;
DROP TABLE IF EXISTS AccessLogs;
DROP TABLE IF EXISTS MaintenanceRequests;
DROP TABLE IF EXISTS Emails;
DROP TABLE IF EXISTS SpecialInstructions;
DROP TABLE IF EXISTS BillingAddressHistory;
DROP TABLE IF EXISTS BillingAddress;
DROP TABLE IF EXISTS CreditCardInfo; 
DROP TABLE IF EXISTS OwnerAnnouncements;
DROP TABLE IF EXISTS TenantAnnouncements;
DROP TABLE IF EXISTS PropertyTenants;
DROP TABLE IF EXISTS PropertyOwners;

-- ✅ Drop parent tables next
DROP TABLE IF EXISTS Owners;
DROP TABLE IF EXISTS Pricing;
DROP TABLE IF EXISTS Tenants;
DROP TABLE IF EXISTS Properties;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Permissions;
DROP TABLE IF EXISTS Roles;
DROP TABLE IF EXISTS Vendors;
DROP TABLE IF EXISTS InvoiceAuditLog;

-- ✅ Drop lookup tables last
DROP TABLE IF EXISTS lkupCategory;
DROP TABLE IF EXISTS lkupPaymentMethods;
DROP TABLE IF EXISTS lkupCreditCards;
DROP TABLE IF EXISTS lkupMaintenanceRequestTypes;
DROP TABLE IF EXISTS lkupPropertyRooms;
DROP TABLE IF EXISTS lkupLineItemType;
DROP TABLE IF EXISTS lkupServiceTypes;
DROP TABLE IF EXISTS lkupInvoiceStatus;
DROP TABLE IF EXISTS LkupUtilities;
DROP TABLE IF EXISTS LkupCleaningType;

DROP VIEW IF EXISTS InvoiceDetailsView
