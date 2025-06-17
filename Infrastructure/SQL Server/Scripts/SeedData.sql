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

-- ✅ Insert seed data into Property table
INSERT INTO [dbo].[Property] ([Name], [Address], [Address1], [City], [State], [PostalCode], [Country], [Bedrooms], [Bathrooms], [SquareFeet], [IsAvailable], [IsActive]) VALUES
('Sunset Villa', '123 Main St', 'Apt 1', 'Los Angeles', 'CA', '90001', 'USA', 3, 2, 1800, 1, 1),
('Ocean Breeze Condo', '456 Beach Ave', 'Unit 5', 'Miami', 'FL', '33101', 'USA', 2, 2, 1200, 1, 1),
('Mountain Retreat', '789 Pine Rd', 'Cabin 3', 'Denver', 'CO', '80201', 'USA', 4, 3, 2500, 1, 1);
GO

-- ✅ Insert seed data into PropertyPhotos table
INSERT INTO [dbo].[PropertyPhotos] ([PropertyId], [PhotoUrl],[Room], [Caption], [UploadedAt]) VALUES
(1, 'https://example.com/photos/sunset_villa.jpg','Front Room', 'Front view of Sunset Villa', GETDATE()),
(2, 'https://example.com/photos/ocean_breeze.jpg', 'Balcony', 'Balcony view of Ocean Breeze Condo', GETDATE()),
(3, 'https://example.com/photos/mountain_retreat.jpg', 'Scenic', 'Scenic view from Mountain Retreat', GETDATE());
GO

-- ✅ Insert seed data into Pricing table
INSERT INTO [dbo].[Pricing] ([PropertyId],[EffectiveDate], [RentalAmount], [DepositAmount], [LeaseTerm], [UtilitiesIncluded]) VALUES
(1,'12/12/2024', 2500.00, 5000.00, '12 Months', 1),
(2,'12/12/2024',1800.00, 3600.00, '6 Months', 0),
(3,'12/12/2024', 3200.00, 6400.00, '24 Months', 1);
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

-- ✅ Insert seed data into Tenants table
INSERT INTO [dbo].[Tenants] ([UserId], [PropertyId], [FirstName], [LastName], [PhoneNumber], [MoveInDate]) VALUES
(3, 1, 'John', 'Doe', '555-1234', '2024-01-15'), -- ✅ Tenant UserId = 3
(5, 2, 'Michael', 'Johnson', '555-9876', '2022-09-20'); -- ✅ Another Tenant UserId = 5
GO


-- ✅ Insert seed data into PaymentMethods table
INSERT INTO [dbo].[lkupPaymentMethods] ([MethodName], [Description], [IsActive]) VALUES
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

INSERT INTO [dbo].[lkupCategory] ([CategoryName])
VALUES 
    ('Lease'),
    ('ID'),
    ('Receipt'),
    ('Maintenance'),
    ('Inspection'),
    ('Notice'),
    ('Insurance'),
    ('Other');
GO
INSERT INTO  [dbo].[lkupCreditCards] ([CreditCardName]) VALUES
    ('Visa'),
    ('MasterCard'),
    ('American Express'),
    ('Discover'),
    ('Diners Club'),
    ('JCB'),
    ('UnionPay'),
	('PayPal'),
	('Coinbase');

GO
INSERT INTO  [dbo].[lkupMaintenanceRequestTypes] ([RequestTypeName], [Description]) VALUES
    ('Plumbing', 'Issues related to leaks, clogged drains, or faulty fixtures'),
    ('Electrical', 'Repair or installation of wiring, outlets, or circuit breakers'),
    ('HVAC', 'Heating, ventilation, and air conditioning system maintenance'),
    ('Roofing', 'Roof inspections, leaks, or repairs'),
    ('Appliance Repair', 'Fixing or replacing household appliances'),
    ('Pest Control', 'Treatment for infestations or prevention measures'),
    ('Landscaping', 'Lawn care, tree trimming, or irrigation system repairs'),
    ('Painting', 'Interior or exterior painting work'),
    ('Security Systems', 'Maintenance of cameras, alarms, or door locks'),
    ('General Repairs', 'Miscellaneous maintenance and minor home fixes');
GO
INSERT INTO  [dbo].[lkupPropertyRooms] ([RoomName], [Description]) VALUES
    ('Living Room', 'Main communal area for relaxation and gatherings'),
    ('Kitchen', 'Area for cooking and food preparation'),
    ('Dining Room', 'Space for eating meals'),
    ('Bedroom', 'Private sleeping area'),
    ('Bathroom', 'Room with bath/shower, sink, and toilet'),
    ('Home Office', 'Dedicated workspace for remote work or study'),
    ('Laundry Room', 'Space for washing and drying clothes'),
    ('Garage', 'Storage and parking area for vehicles'),
    ('Basement', 'Underground storage or living space'),
    ('Attic', 'Top-floor storage space'),
    ('Hallway', 'Connecting space between rooms'),
    ('Closet', 'Storage space for clothing and items'),
    ('Sunroom', 'Glass-enclosed space for natural light exposure'),
    ('Pantry', 'Small storage space for food and kitchen essentials'),
    ('Mudroom', 'Entryway for shoes, coats, and outdoor gear'),
    ('Guest Room', 'Dedicated sleeping space for visitors'),
    ('Playroom', 'Space for children’s activities and toys'),
    ('Gym', 'Home fitness area'),
    ('Library', 'Room for books, reading, and study');
GO
-- Seed Data for Common Invoice Types
INSERT INTO [dbo].[lkupInvoiceType] ([InvoiceTypeName], [Description]) VALUES 
('Rent', 'Monthly rental payment for tenants'),
('Maintenance', 'Charges for property maintenance and repairs'),
('Utilities', 'Water, electricity, gas, and other utility bills'),
('Security Deposit', 'Refundable deposit for new tenants'),
('Late Fee', 'Penalty for overdue payments'),
('Parking Fee', 'Charges for parking space usage'),
('HOA Fees', 'Homeowners Association fees for shared property management'),
('Cleaning Fee', 'Charges for cleaning services'),
('Lease Termination', 'Fees for early lease termination'),
('Miscellaneous', 'Other property-related charges');

GO
INSERT INTO [dbo].[lkupServiceTypes] ([TypeName]) VALUES
('IT Services'),
('Legal Consulting'),
('Marketing'),
('Financial Advisory'),
('Logistics'),
('Healthcare Services'),
('Construction'),
('Education'),
('Retail'),
('Manufacturing'),
('Handy Man');
GO
INSERT INTO [dbo].[lkupInvoiceType] ([InvoiceType], [Description])
VALUES 
    ('Rent', 'Monthly rent payment from tenants'),
    ('Maintenance', 'Charges for maintenance and repairs'),
    ('Utilities', 'Utility bills such as water, gas, or electricity'),
    ('PropertyTax', 'Annual or quarterly property tax assessments'),
    ('Insurance', 'Insurance premium payments for the property'),
    ('HOAFees', 'Homeowners Association fees'),
    ('SecurityDeposit', 'Initial security deposit from tenants'),
    ('LateFees', 'Fees for overdue rent or payments'),
    ('ParkingFees', 'Charges for tenant parking spaces'),
    ('CleaningFees', 'Charges for professional cleaning services'),
    ('LeaseTermination', 'Fees related to early lease termination'),
    ('LegalFees', 'Legal service charges for disputes or contracts');
