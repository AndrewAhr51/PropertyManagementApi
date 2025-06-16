USE [PropertyManagement];
GO

-- ✅ Drop tables with foreign key dependencies first
DROP TABLE IF EXISTS [dbo].[lkupCategory];
DROP TABLE IF EXISTS [dbo].[PaymentReminders];
DROP TABLE IF EXISTS [dbo].[Documents];
DROP TABLE IF EXISTS [dbo].[Notes];
DROP TABLE IF EXISTS [dbo].[Invoices];
DROP TABLE IF EXISTS [dbo].[Leases];
DROP TABLE IF EXISTS [dbo].[PropertyPhotos];
DROP TABLE IF EXISTS [dbo].[AccessLogs];
DROP TABLE IF EXISTS [dbo].[MaintenanceRequests];
DROP TABLE IF EXISTS [dbo].[Emails];
DROP TABLE IF EXISTS [dbo].[SpecialInstructions];
DROP TABLE IF EXISTS [dbo].[BillingAddress];
DROP TABLE IF EXISTS [dbo].[CreditCardInfo];
DROP TABLE IF EXISTS [dbo].[Payments];
DROP TABLE IF EXISTS [dbo].[lkupPaymentMethods];
DROP TABLE IF EXISTS [dbo].[PropertyOwners];
DROP TABLE IF EXISTS [dbo].[Owners];
DROP TABLE IF EXISTS [dbo].[Pricing];
DROP TABLE IF EXISTS [dbo].[Property];
DROP TABLE IF EXISTS [dbo].[Tenants];
DROP TABLE IF EXISTS [dbo].[Users];
DROP TABLE IF EXISTS [dbo].[RolePermissions];
DROP TABLE IF EXISTS [dbo].[Permissions];
DROP TABLE IF EXISTS [dbo].[Roles];
DROP TABLE IF EXISTS [dbo].[Vendors];
DROP TABLE IF EXISTS [dbo].[lkupCreditCards];
DROP TABLE IF EXISTS [dbo].[lkupMaintenanceRequestTypes];
DROP TABLE IF EXISTS [dbo].[lkupPropertyRooms];
DROP TABLE IF EXISTS [dbo].[lkupInvoiceType];
DROP TABLE IF EXISTS [dbo].[lkupServiceTypes];
DROP TABLE IF EXISTS [dbo].[lkupServiceTypes];
GO