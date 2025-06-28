USE PropertyManagement;
DROP TRIGGER IF EXISTS after_invoice_insert;
DROP TRIGGER IF EXISTS after_invoice_update;
DROP TRIGGER IF EXISTS before_invoice_delete;
DROP TRIGGER IF EXISTS audit_invoice_insert;
DROP TRIGGER IF EXISTS audit_invoice_update;
DROP TRIGGER IF EXISTS audit_invoice_delete;
DROP TRIGGER IF EXISTS LogBillingAddressInsert;
DROP TRIGGER IF EXISTS LogBillingAddressUpdate;
DROP TRIGGER IF EXISTS LogBillingAddressDelete;

-- ✅ Drop tables with foreign key dependencies first
DROP TABLE IF EXISTS InvoiceAuditLog;
DROP TABLE IF EXISTS RolePermissions;
DROP TABLE IF EXISTS PaymentReminders;
DROP TABLE IF EXISTS DocumentStorage;
DROP TABLE IF EXISTS Documents;
DROP TABLE IF EXISTS Notes;

-- ✅ Drop invoice subtype tables before Invoices
DROP TABLE IF EXISTS LegalFeeInvoices;
DROP TABLE IF EXISTS InsuranceInvoices;
DROP TABLE IF EXISTS PropertyTaxInvoices;
DROP TABLE IF EXISTS ParkingFeeInvoices;
DROP TABLE IF EXISTS LeaseTerminationInvoices;
DROP TABLE IF EXISTS CleaningFeeInvoices;
DROP TABLE IF EXISTS SecurityDepositInvoices;
DROP TABLE IF EXISTS UtilityInvoices;
DROP TABLE IF EXISTS RentInvoices;
DROP TABLE IF EXISTS TenantBankAccount;
DROP TABLE IF EXISTS OwnerBankAccount; 
DROP TABLE IF EXISTS PreferredMethod;
DROP TABLE IF EXISTS CardToken; 
DROP TABLE IF EXISTS BankAccountInfo;
DROP TABLE IF EXISTS SpecialInstructions;
DROP TABLE IF EXISTS CheckPayments;
DROP TABLE IF EXISTS WireTransfers;
DROP TABLE IF EXISTS CreditCardPayments;
DROP TABLE IF EXISTS Payments;

-- ✅ Drop base invoice table
DROP TABLE IF EXISTS Invoices;

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
DROP TABLE IF EXISTS lkupInvoiceType;
DROP TABLE IF EXISTS lkupServiceTypes;
DROP TABLE IF EXISTS lkupInvoiceStatus;
DROP TABLE IF EXISTS LkupUtilities;
DROP TABLE IF EXISTS LkupCleaningType;
