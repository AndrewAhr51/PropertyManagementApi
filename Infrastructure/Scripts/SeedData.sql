INSERT INTO [dbo].[Roles] ([Name], [Description]) VALUES
('Admin', 'Full access to all features'),
('Manager', 'Manages properties and tenants'),
('Tenant', 'Limited access to own records');
GO

INSERT INTO [dbo].[Permissions] ([Name], [Description]) VALUES
('ViewDashboard', 'Access to admin dashboard'),
('ManageProperties', 'Create and edit property listings'),
('ProcessPayments', 'Handle and track rent payments');
GO

INSERT INTO [dbo].[RolePermissions] ([RoleId], [PermissionId]) VALUES
(1,1), (1,2), (1,3),
(2,2), (2,3),
(3,3);
GO
INSERT INTO [dbo].[Users] ([Username], [Email], [PasswordHash], [RoleId]) VALUES
('admin_user', 'admin@propertyco.com', 'hashed_pw_admin', 1),
('property_mgr', 'manager@propertyco.com', 'hashed_pw_mgr', 2),
('jane.tenant', 'jane@tenantmail.com', 'hashed_pw_jane', 3),
('john.tenant', 'john@tenantmail.com', 'hashed_pw_john', 3);
GO

INSERT INTO [dbo].[Tenants] ([UserId], [FirstName], [LastName], [PhoneNumber], [MoveInDate]) VALUES
(3, 'Jane', 'Tenant', '123-456-7890', '2024-05-01'),
(4, 'John', 'Tenant', '987-654-3210', '2024-06-01');
GO	
INSERT INTO [dbo].[Properties] ([Name], [Address], [City], [State], [PostalCode], [Country], [Bedrooms], [Bathrooms], [SquareFeet])
VALUES 
('Maplewood Villa', '123 Elm Street', 'Orlando', 'FL', '32801', 'USA', 2, 2, 1100),
('Oakwood Apartments', '456 Oak Avenue', 'Miami', 'FL', '33101', 'USA', 3, 2, 1400);
GO

INSERT INTO [dbo].[Pricing] ([PropertyId], [RentalAmount], [DepositAmount], [LeaseTerm], [UtilitiesIncluded])
VALUES 
(1, 1850.00, 1200.00, '12 Months', 1),
(2, 2200.00, 1500.00, '12 Months', 0);
GO
INSERT INTO [dbo].[PaymentMethods] ([MethodName], [Description], [IsActive]) VALUES
('Credit Card', 'Standard credit/debit card payment', 1),
('ACH Transfer', 'Bank-to-bank payment', 1);
GO

INSERT INTO [dbo].[Payments] ([TenantId], [PropertyId], [Amount], [PaymentMethodId], [TransactionDate], [ReferenceNumber])
VALUES 
(1, 1, 1850.00, 1, GETDATE(), 'PMT1001'),
(2, 2, 2200.00, 2, GETDATE(), 'PMT1002');
GO 
INSERT INTO [dbo].[CreditCardInfo] ([TenantId], [PropertyId], [CardHolderName], [CardNumber], [ExpirationDate], [CVV], [CreatedAt]) 
VALUES 
(1, 1, 'Jane Tenant', CONVERT(VARBINARY, '4111111111111111'), '2026-12-01', CONVERT(VARBINARY, '123'), GETDATE()),
(2, 2, 'John Doe', CONVERT(VARBINARY, '5500000000000004'), '2025-08-01', CONVERT(VARBINARY, '456'), GETDATE());
GO

INSERT INTO [dbo].[BillingAddress] ([CardId], [StreetAddressLine1], [StreetAddressLine2], [City], [State], [PostalCode], [Country]) 
VALUES 
(1, '123 Elm Street', NULL, 'Orlando', 'FL', '32801', 'USA'),
(2, '456 Oak Avenue', 'Apt 3B', 'Miami', 'FL', '33101', 'USA');
GO
INSERT INTO [dbo].[Leases] ([TenantId], [PropertyId], [StartDate], [MonthlyRent], [DepositPaid])
VALUES 
(1, 1, '2024-05-01', 1850.00, 1),
(2, 2, '2024-06-01', 2200.00, 1);
GO

INSERT INTO [dbo].[Invoices] ([TenantId], [PropertyId], [AmountDue], [DueDate], [IsPaid], [CreatedAt])
VALUES 
(1, 1, 1850.00, '2024-07-01', 0, GETDATE()),
(2, 2, 2200.00, '2024-08-01', 0, GETDATE());
GO
	
INSERT INTO [dbo].[PaymentReminders] ([TenantId], [PropertyId], [InvoiceId], [ReminderDate], [Status]) 
VALUES 
(1, 1, 1, DATEADD(DAY, -7, '2024-07-01'), 'Pending'),
(2, 2, 2, DATEADD(DAY, -3, '2024-08-01'), 'Pending');
GO
INSERT INTO [dbo].[Emails] ([SenderId], [Recipient], [Subject], [Body], [SentDate], [Status], [IsDelivered]) 
VALUES  
(1, 'jane@tenantmail.com', 'Welcome to Maplewood', 'We’re thrilled to have you!', GETUTCDATE(), 'Pending', 0),
(2, 'john@tenantmail.com', 'Lease Agreement Update', 'Please review your updated lease terms.', GETUTCDATE(), 'Pending', 0);
GO
GO

INSERT INTO [dbo].[MaintenanceRequests] ([TenantId], [PropertyId], [Category], [Description], [PriorityLevel])
VALUES 
(1, 1, 'Plumbing', 'Leaky faucet in kitchen', 'Normal'),
(2, 2, 'Electrical', 'Light fixture not working in living room', 'High');
GO
INSERT INTO [dbo].[Vendors] ([Name], [ServiceType], [ContactEmail], [PhoneNumber]) 
VALUES 
('Fast Fix Plumbing', 'Plumbing', 'support@fastfix.com', '800-555-1234'),
('Electricians Plus', 'Electrical', 'contact@electriciansplus.com', '800-555-5678');
GO

INSERT INTO [dbo].[PropertyPhotos] ([PropertyId], [PhotoUrl], [Caption])
VALUES 
(1, 'https://example.com/photos/elm_st_1.jpg', 'Front view of Maplewood Villa'),
(2, 'https://example.com/photos/oak_apt_1.jpg', 'Living room with large windows');
GO
INSERT INTO [dbo].[AccessLogs] ([UserId], [Action], [Timestamp]) 
VALUES 
(1, 'Logged in', GETDATE()),
(2, 'Viewed property details', GETDATE());
GO

INSERT INTO [dbo].[Notes] ([CreatedBy], [TenantId], [PropertyId], [NoteText])
VALUES 
(1, 1, 1, 'Tenant requested early move-in.'),
(2, 2, 2, 'Tenant inquired about pet policy.');
GO

