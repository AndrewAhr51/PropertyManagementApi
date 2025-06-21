USE propertymanagement;
-- ✅ Insert seed data into lkupCategory table
INSERT INTO lkupCategory (CategoryName) VALUES
('Lease'),
('ID'),
('Receipt'),
('Maintenance'),
('Inspection'),
('Notice'),
('Insurance'),
('Other');

-- ✅ Insert seed data into PaymentMethods table
INSERT INTO lkupPaymentMethods (MethodName, Description, IsActive) VALUES
('Credit Card', 'Payments made via credit card', TRUE),
('Bank Transfer', 'Direct bank transfer payments', TRUE),
('PayPal', 'Online payments via PayPal', TRUE),
('Cash', 'Physical cash payments', TRUE);

-- ✅ Insert seed data into lkupCreditCards table
INSERT INTO lkupCreditCards (CreditCardName) VALUES
('Visa'),
('MasterCard'),
('American Express'),
('Discover'),
('Diners Club'),
('JCB'),
('UnionPay'),
('PayPal'),
('Coinbase');

-- ✅ Insert seed data into lkupMaintenanceRequestTypes table
INSERT INTO lkupMaintenanceRequestTypes (RequestTypeName, Description) VALUES
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

-- ✅ Insert seed data into lkupPropertyRooms table
INSERT INTO lkupPropertyRooms (RoomName, Description) VALUES
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

-- ✅ Insert seed data into lkupServiceTypes table
INSERT INTO lkupServiceTypes (TypeName) VALUES
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

-- ✅ Insert seed data into lkupInvoiceType table (alternative format)
INSERT INTO lkupInvoiceType (InvoiceType, Description) VALUES
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

-- ✅ Insert seed data into Roles table
INSERT INTO Roles (Name, Description) VALUES
('Admin', 'Has full access to all system features'),
('Manager', 'Can manage properties and tenants'),
('Tenant', 'Limited access to personal lease and payment details'),
('Owner', 'Can view and manage owned properties');

-- ✅ Insert seed data into Permissions table
INSERT INTO Permissions (Name, Description) VALUES
('ViewProperties', 'Allows viewing property details'),
('EditProperties', 'Allows editing property details'),
('DeleteProperties', 'Allows deleting properties'),
('ManageUsers', 'Allows managing user accounts'),
('ViewPayments', 'Allows viewing payment history'),
('ProcessPayments', 'Allows processing payments'),
('ManageLeases', 'Allows managing lease agreements');

-- ✅ Insert seed data into RolePermissions table (Associating Roles with Permissions)
INSERT INTO RolePermissions (RoleId, PermissionId) VALUES
-- Admin has all permissions
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7),
-- Manager can manage properties, leases, and payments
(2, 1), (2, 2), (2, 5), (2, 6), (2, 7),
-- Tenant can only view properties and payments
(3, 1), (3, 5),
-- Owner can view and edit their properties
(4, 1), (4, 2);

-- ✅ Insert seed data into Users table
INSERT INTO Users (UserName, Email, PasswordHash, RoleId, CreatedBy, IsMfaEnabled, IsActive) VALUES
('admin_user', 'admin@example.com', 'hashed_password_1', 1, CURRENT_TIMESTAMP, TRUE, TRUE),
('manager_user', 'manager@example.com', 'hashed_password_2', 2, CURRENT_TIMESTAMP, TRUE, TRUE),
('john_doe', 'tenant@example.com', 'hashed_password_3', 3, CURRENT_TIMESTAMP, TRUE, TRUE),
('jane_smith', 'owner@example.com', 'hashed_password_4', 4, CURRENT_TIMESTAMP, TRUE, TRUE),
('michael_johnson', 'michael.johnson@example.com', 'hashed_password_5', 3, CURRENT_TIMESTAMP, TRUE, TRUE);

-- ✅ Insert seed data into Property table
INSERT INTO Property (Name, Address, Address1, City, State, PostalCode, Country, Bedrooms, Bathrooms, SquareFeet, IsAvailable, IsActive) VALUES
('Sunset Villa', '123 Main St', 'Apt 1', 'Los Angeles', 'CA', '90001', 'USA', 3, 2, 1800, TRUE, TRUE),
('Ocean Breeze Condo', '456 Beach Ave', 'Unit 5', 'Miami', 'FL', '33101', 'USA', 2, 2, 1200, TRUE, TRUE),
('Mountain Retreat', '789 Pine Rd', 'Cabin 3', 'Denver', 'CO', '80201', 'USA', 4, 3, 2500, TRUE, TRUE);

-- ✅ Insert seed data into PropertyPhotos table
INSERT INTO PropertyPhotos (PropertyId, PhotoUrl, Room, Caption, CreatedDate) VALUES
(1, 'https://example.com/photos/sunset_villa.jpg', 'Front Room', 'Front view of Sunset Villa', CURRENT_TIMESTAMP),
(2, 'https://example.com/photos/ocean_breeze.jpg', 'Balcony', 'Balcony view of Ocean Breeze Condo', CURRENT_TIMESTAMP),
(3, 'https://example.com/photos/mountain_retreat.jpg', 'Scenic', 'Scenic view from Mountain Retreat', CURRENT_TIMESTAMP);

-- ✅ Insert seed data into Pricing table
INSERT INTO Pricing (PropertyId, EffectiveDate, RentalAmount, DepositAmount, LeaseTerm, UtilitiesIncluded) VALUES
(1, '2024-12-12', 2500.00, 5000.00, '12 Months', TRUE),
(2, '2024-12-12', 1800.00, 3600.00, '6 Months', FALSE),
(3, '2024-12-12', 3200.00, 6400.00, '24 Months', TRUE);

-- ✅ Insert seed data into Owners table
INSERT INTO Owners (FirstName, LastName, Email, Phone, Address1, Address2, City, State, PostalCode, Country, IsActive) VALUES
('Alice', 'Johnson', 'alice.johnson@example.com', '555-1234', '789 Oak St', 'Suite 5', 'Chicago', 'IL', '60601', 'USA', TRUE),
('Bob', 'Williams', 'bob.williams@example.com', '555-5678', '456 Maple Ave', NULL, 'Seattle', 'WA', '98101', 'USA', TRUE),
('Charlie', 'Brown', 'charlie.brown@example.com', '555-9876', '123 Pine Rd', 'Apt 2B', 'Denver', 'CO', '80201', 'USA', TRUE);

-- ✅ Insert seed data into PropertyOwners table
INSERT INTO PropertyOwners (PropertyId, OwnerId, OwnershipPercentage) VALUES
(1, 1, 100.00),
(2, 2, 100.00),
(3, 3, 100.00);

-- ✅ Insert seed data into Tenants table
INSERT INTO Tenants (UserId, PropertyId, FirstName, LastName, PhoneNumber, MoveInDate) VALUES
(3, 1, 'John', 'Doe', '555-1234', '2024-01-15'),
(5, 2, 'Michael', 'Johnson', '555-9876', '2022-09-20');

-- ✅ Insert seed data into Payments table
INSERT INTO Payments (TenantId, PropertyId, Amount, PaymentMethodId, TransactionDate, ReferenceNumber) VALUES
(1, 1, 2500.00, 1, CURRENT_TIMESTAMP, 'REF123456'),
(2, 3, 3200.00, 3, CURRENT_TIMESTAMP, 'REF345678');
