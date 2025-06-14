USE [PropertyManagement];
GO

-- Dynamically drop all foreign key constraints
DECLARE @sql NVARCHAR(MAX) = '';

SELECT @sql = STRING_AGG('ALTER TABLE ' + TABLE_NAME + ' DROP CONSTRAINT ' + CONSTRAINT_NAME, '; ')
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE CONSTRAINT_TYPE = 'FOREIGN KEY';

IF @sql IS NOT NULL
    EXEC sp_executesql @sql;
GO

-- Drop child tables first
DROP TABLE IF EXISTS [dbo].[Documents];
DROP TABLE IF EXISTS [dbo].[Notes];
DROP TABLE IF EXISTS [dbo].[PaymentReminders];
DROP TABLE IF EXISTS [dbo].[Invoices];
DROP TABLE IF EXISTS [dbo].[Payments];
DROP TABLE IF EXISTS [dbo].[CreditCardInfo];
DROP TABLE IF EXISTS [dbo].[BillingAddress];
DROP TABLE IF EXISTS [dbo].[MaintenanceRequests];
DROP TABLE IF EXISTS [dbo].[Emails];
DROP TABLE IF EXISTS [dbo].[SpecialInstructions];
DROP TABLE IF EXISTS [dbo].[AccessLogs];
DROP TABLE IF EXISTS [dbo].[PropertyPhotos];

-- Drop reference tables
DROP TABLE IF EXISTS [dbo].[Pricing];
DROP TABLE IF EXISTS [dbo].[Leases];
DROP TABLE IF EXISTS [dbo].[PaymentMethods];

-- Drop core entity tables
DROP TABLE IF EXISTS [dbo].[Tenants];
DROP TABLE IF EXISTS [dbo].[Properties];
DROP TABLE IF EXISTS [dbo].[Users];

-- Drop lookup tables
DROP TABLE IF EXISTS [dbo].[RolePermissions];
DROP TABLE IF EXISTS [dbo].[Permissions];
DROP TABLE IF EXISTS [dbo].[Roles];

-- Drop supporting business tables
DROP TABLE IF EXISTS [dbo].[Vendors];
GO