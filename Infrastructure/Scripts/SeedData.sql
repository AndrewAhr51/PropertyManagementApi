USE [PropertyManagement];
GO

-- ✅ Insert seed data into Roles table
INSERT INTO [dbo].[Roles] ([Name], [Description]) VALUES
('Admin', 'Has full access to all system features'),
('Manager', 'Can manage properties and tenants'),
('Tenant', 'Limited access to personal lease and payment details'),
('Owner', 'Can view and manage owned properties');
GO

-- ✅ Insert seed data into Permissions table
INSERT INTO [dbo].[Permissions] ([Name], [Description]) VALUES
('ViewProperties', 'Allows viewing property details'),
('EditProperties', 'Allows editing property details'),
('DeleteProperties', 'Allows deleting properties'),
('ManageUsers', 'Allows managing user accounts'),
('ViewPayments', 'Allows viewing payment history'),
('ProcessPayments', 'Allows processing payments'),
('ManageLeases', 'Allows managing lease agreements');
GO

-- ✅ Insert seed data into RolePermissions table (Associating Roles with Permissions)
INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId]) VALUES
-- Admin has all permissions
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7),

-- Manager can manage properties, leases, and payments
(2, 1), (2, 2), (2, 5), (2, 6), (2, 7),

-- Tenant can only view properties and payments
(3, 1), (3, 5),

-- Owner can view and edit their properties
(4, 1), (4, 2);
GO

-- ✅ Insert seed data into Users table
INSERT INTO [dbo].[Users] ([UserName], [Email], [PasswordHash], [RoleId], [CreatedAt], [IsMfaEnabled], [IsActive]) VALUES
('admin_user', 'admin@example.com', 'hashed_password_1', 1, GETUTCDATE(), 1, 1),
('manager_user', 'manager@example.com', 'hashed_password_2', 2, GETUTCDATE(), 1, 1),
('john_doe', 'tenant@example.com', 'hashed_password_3', 3, GETUTCDATE(), 1, 1),
('jane_smith', 'owner@example.com', 'hashed_password_4', 4, GETUTCDATE(), 1, 1),
('michael_johnson', 'michael.johnson@example.com', 'hashed_password_5', 3, GETUTCDATE(), 1, 1);
GO

-- ✅ Insert seed data into Tenants table
INSERT INTO [dbo].[Tenants] ([UserId], [FirstName], [LastName], [PhoneNumber], [MoveInDate]) VALUES
(3, 'John', 'Doe', '555-1234', '2024-01-15'), -- ✅ Tenant UserId = 3
(5, 'Michael', 'Johnson', '555-9876', '2022-09-20'); -- ✅ Another Tenant UserId = 5
GO

-- ✅ Insert seed data into Property table
INSERT INTO [dbo].[Property] ([Name], [Address], [Address1], [City], [State], [PostalCode], [Country], [Bedrooms], [Bathrooms], [SquareFeet], [IsAvailable], [IsActive]) VALUES
('Sunset Villa', '123 Main St', 'Apt 1', 'Los Angeles', 'CA', '90001', 'USA', 3, 2, 1800, 1, 1),
('Ocean Breeze Condo', '456 Beach Ave', 'Unit 5', 'Miami', 'FL', '33101', 'USA', 2, 2, 1200, 1, 1),
('Mountain Retreat', '789 Pine Rd', 'Cabin 3', 'Denver', 'CO', '80201', 'USA', 4, 3, 2500, 1, 1);
GO

-- ✅ Insert seed data into PropertyPhotos table
INSERT INTO [dbo].[PropertyPhotos] ([PropertyId], [PhotoUrl], [Caption], [UploadedAt]) VALUES
(1, 'https://example.com/photos/sunset_villa.jpg', 'Front view of Sunset Villa', GETDATE()),
(2, 'https://example.com/photos/ocean_breeze.jpg', 'Balcony view of Ocean Breeze Condo', GETDATE()),
(3, 'https://example.com/photos/mountain_retreat.jpg', 'Scenic view from Mountain Retreat', GETDATE());
GO

-- ✅ Insert seed data into Pricing table
INSERT INTO [dbo].[Pricing] ([PropertyId], [RentalAmount], [DepositAmount], [LeaseTerm], [UtilitiesIncluded]) VALUES
(1, 2500.00, 5000.00, '12 Months', 1),
(2, 1800.00, 3600.00, '6 Months', 0),
(3, 3200.00, 6400.00, '24 Months', 1);
GO

-- ✅ Insert seed data into Owners table
INSERT INTO [dbo].[Owners] ([FirstName], [LastName], [Email], [Phone], [Address1], [Address2], [City], [State], [PostalCode], [Country], [IsActive]) VALUES
('Alice', 'Johnson', 'alice.johnson@example.com', '555-1234', '789 Oak St', 'Suite 5', 'Chicago', 'IL', '60601', 'USA',1),
('Bob', 'Williams', 'bob.williams@example.com', '555-5678', '456 Maple Ave', NULL, 'Seattle', 'WA', '98101', 'USA',1),
('Charlie', 'Brown', 'charlie.brown@example.com', '555-9876', '123 Pine Rd', 'Apt 2B', 'Denver', 'CO', '80201', 'USA',1);
GO

-- ✅ Insert seed data into PropertyOwners table
INSERT INTO [dbo].[PropertyOwners] ([PropertyId], [OwnerId], [OwnershipPercentage]) VALUES
(1, 1, 100.00),
(2, 2, 100.00),
(3, 3, 100.00);
GO

-- ✅ Insert seed data into PaymentMethods table
INSERT INTO [dbo].[PaymentMethods] ([MethodName], [Description], [IsActive]) VALUES
('Credit Card', 'Payments made via credit card', 1),
('Bank Transfer', 'Direct bank transfer payments', 1),
('PayPal', 'Online payments via PayPal', 1),
('Cash', 'Physical cash payments', 1);
GO

-- ✅ Insert seed data into Payments table
INSERT INTO [dbo].[Payments] ([TenantId], [PropertyId], [Amount], [PaymentMethodId], [TransactionDate], [ReferenceNumber]) VALUES
(1, 1, 2500.00, 1, GETDATE(), 'REF123456'),
(2, 3, 3200.00, 3, GETDATE(), 'REF345678');
GO

-- ✅ Insert seed data into CreditCardInfo table
INSERT INTO [dbo].[CreditCardInfo] ([TenantId], [PropertyId], [CardHolderName], [CardNumber], [ExpirationDate], [CVV], [CreatedAt]) VALUES
(1, 1, 'John Doe', CONVERT(VARBINARY(256), '4111111111111111'), '2026-05-01', CONVERT(VARBINARY(256), '123'), GETDATE()),
(2, 3, 'Michael Johnson', CONVERT(VARBINARY(256), '378282246310005'), '2025-12-10', CONVERT(VARBINARY(256), '789'), GETDATE());
GO