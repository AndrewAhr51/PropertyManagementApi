USE PropertyManagement;

-- ✅ Drop tables with foreign key dependencies first
USE PropertyManagement;

-- ✅ Drop tables with foreign key dependencies first
DROP TABLE IF EXISTS RolePermissions;
DROP TABLE IF EXISTS PaymentReminders;
DROP TABLE IF EXISTS Documents;
DROP TABLE IF EXISTS Notes;
DROP TABLE IF EXISTS Invoices;
DROP TABLE IF EXISTS Leases;
DROP TABLE IF EXISTS PropertyPhotos;
DROP TABLE IF EXISTS AccessLogs;
DROP TABLE IF EXISTS MaintenanceRequests;
DROP TABLE IF EXISTS Emails;
DROP TABLE IF EXISTS SpecialInstructions;
DROP TABLE IF EXISTS BillingAddress;
DROP TABLE IF EXISTS CreditCardInfo;
DROP TABLE IF EXISTS Payments;
DROP TABLE IF EXISTS PropertyOwners;

-- ✅ Drop parent tables next
DROP TABLE IF EXISTS Owners;
DROP TABLE IF EXISTS Pricing;
DROP TABLE IF EXISTS Tenants;
DROP TABLE IF EXISTS PropertyPhotos;
DROP TABLE IF EXISTS Property;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Permissions;
DROP TABLE IF EXISTS Roles;
DROP TABLE IF EXISTS Vendors;

-- ✅ Drop lookup tables last
DROP TABLE IF EXISTS lkupCategory;
DROP TABLE IF EXISTS lkupPaymentMethods;
DROP TABLE IF EXISTS lkupCreditCards;
DROP TABLE IF EXISTS lkupMaintenanceRequestTypes;
DROP TABLE IF EXISTS lkupPropertyRooms;
DROP TABLE IF EXISTS lkupInvoiceType;
DROP TABLE IF EXISTS lkupServiceTypes;