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

INSERT INTO lkupInvoiceStatus (Id, Name) VALUES
    (1, 'Pending'),
    (2, 'Paid'),
    (3, 'Overdue'),
    (4, 'Cancelled');
    
INSERT INTO LkupUtilities (UtilityName, Description) VALUES
('Electricity', 'Electric power usage and billing'),
('Water', 'Water supply and consumption'),
('Gas', 'Natural gas usage'),
('Trash', 'Garbage collection services'),
('Internet', 'Broadband or fiber internet service'),
('Sewer', 'Sewage and wastewater services'),
('Recycling', 'Recyclable waste collection'),
('Cable TV', 'Television cable service');

INSERT INTO LkupCleaningType (CleaningTypeName, Description) VALUES
('Move-In Cleaning', 'Deep cleaning before a new tenant moves in'),
('Move-Out Cleaning', 'Thorough cleaning after a tenant vacates'),
('Routine Cleaning', 'Scheduled maintenance cleaning'),
('Emergency Cleaning', 'Unplanned cleaning due to damage or incident'),
('Post-Renovation Cleaning', 'Cleanup after construction or remodeling'),
('Carpet Cleaning', 'Professional carpet shampooing or steaming'),
('Window Cleaning', 'Interior and exterior window washing'),
('Appliance Cleaning', 'Detailed cleaning of kitchen and laundry appliances');

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

-- ✅ Insert seed data into Property table
INSERT INTO Properties (PropertyName, Address, Address1, City, State, PostalCode, Country, Bedrooms, Bathrooms, SquareFeet, PropertyTaxes, Insurance, IsAvailable, IsActive) VALUES
('Sunset Villa', '123 Main St', 'Apt 1', 'Los Angeles', 'CA', '90001', 'USA', 3, 2, 1800, 2000, 1200,TRUE, TRUE),
('Ocean Breeze Condo', '456 Beach Ave', 'Unit 5', 'Miami', 'FL', '33101', 'USA', 2, 2, 1200, 3000, 1300,TRUE, TRUE),
('Mountain Retreat', '789 Pine Rd', 'Cabin 3', 'Denver', 'CO', '80201', 'USA', 4, 3, 2500, 4000, 2000,TRUE, TRUE);

-- ✅ Insert seed data into PropertyPhotos table
INSERT INTO PropertyPhotos (PropertyId, PhotoUrl, Room, Caption, UploadedAt) VALUES
(1, 'https://example.com/photos/sunset_villa.jpg', 'Front Room', 'Front view of Sunset Villa', CURRENT_TIMESTAMP),
(2, 'https://example.com/photos/ocean_breeze.jpg', 'Balcony', 'Balcony view of Ocean Breeze Condo', CURRENT_TIMESTAMP),
(3, 'https://example.com/photos/mountain_retreat.jpg', 'Scenic', 'Scenic view from Mountain Retreat', CURRENT_TIMESTAMP);

-- ✅ Insert seed data into Pricing table
INSERT INTO Pricing (PropertyId, EffectiveDate, RentalAmount, DepositAmount, LeaseTerm, UtilitiesIncluded) VALUES
(1, '2024-12-12', 2500.00, 5000.00, '12 Months', TRUE),
(2, '2024-12-12', 1800.00, 3600.00, '6 Months', FALSE),
(3, '2024-12-12', 3200.00, 6400.00, '24 Months', TRUE);

-- ✅ Insert Users (admin, manager, 1 owner, 4 tenants)
INSERT INTO Users (UserName, Email, PasswordHash, RoleId, CreatedDate, IsMfaEnabled, IsActive) VALUES
('admin_user', 'admin@example.com', 'hashed_password_1', 1, CURRENT_TIMESTAMP, TRUE, TRUE),   -- Id = 1
('manager_user', 'manager@example.com', 'hashed_password_2', 2, CURRENT_TIMESTAMP, TRUE, TRUE), -- Id = 2
('alice_johnson', 'alice.johnson@example.com', 'hashed_password_3', 4, CURRENT_TIMESTAMP, TRUE, TRUE), -- Id = 3
('bob_williams', 'bob.williams@example.com', 'hashed_password_4', 4, CURRENT_TIMESTAMP, TRUE, TRUE),   -- Id = 4
('charlie_brown', 'charlie.brown@example.com', 'hashed_password_5', 4, CURRENT_TIMESTAMP, TRUE, TRUE), -- Id = 5
('john_doe', 'john.doe@example.com', 'hashed_password_6', 3, CURRENT_TIMESTAMP, TRUE, TRUE),           -- Id = 6
('michael_johnson', 'michael.johnson@example.com', 'hashed_password_7', 3, CURRENT_TIMESTAMP, TRUE, TRUE), -- Id = 7
('mary_johnson', 'mary.johnson@example.com', 'hashed_password_8', 3, CURRENT_TIMESTAMP, TRUE, TRUE),      -- Id = 8
('john_smith', 'john.smith@example.com', 'hashed_password_9', 3, CURRENT_TIMESTAMP, TRUE, TRUE);          -- Id = 9

-- ✅ Insert matching Owners (OwnerId = Users.Id)
INSERT INTO Owners (
  OwnerId, PrimaryOwner, FirstName, LastName, Email, Phone,
  Address1, Address2, City, State, PostalCode, Country, IsActive
) VALUES
(3, 1, 'Alice', 'Johnson', 'alice.johnson@example.com', '555-1234', '789 Oak St', 'Suite 5', 'Chicago', 'IL', '60601', 'USA', TRUE),
(4, 1, 'Bob', 'Williams', 'bob.williams@example.com', '555-5678', '456 Maple Ave', NULL, 'Seattle', 'WA', '98101', 'USA', TRUE),
(5, 1, 'Charlie', 'Brown', 'charlie.brown@example.com', '555-9876', '123 Pine Rd', 'Apt 2B', 'Denver', 'CO', '80201', 'USA', TRUE);

-- ✅ Insert into PropertyOwners
INSERT INTO PropertyOwners (PropertyId, OwnerId, OwnershipPercentage) VALUES
(1, 3, 100.00),
(2, 4, 100.00),
(3, 5, 100.00);

-- ✅ Insert Tenants (UserId = TenantId)
INSERT INTO Tenants (
  TenantId, PropertyId, PrimaryTenant,
  FirstName, LastName, Email, PhoneNumber, isActive, MoveInDate
) VALUES
(6, 1, 1, 'John', 'Doe', 'john.doe@example.com', '555-1234', 1,'2024-01-15'),
(7, 2, 1, 'Michael', 'Johnson', 'michael.johnson@example.com', '555-9876', 1,'2022-09-20'),
(8, 2, 0, 'Mary', 'Johnson', 'mary.johnson@example.com', '555-9876', 1,'2022-09-20'),
(9, 3, 1, 'John', 'Smith', 'john.smith@example.com', '555-5555', 1,'2022-10-01');

INSERT INTO PropertyTenants (PropertyId, TenantId) VALUES
(1, 6),
(2, 7),
(2, 8),
(3, 9);

-- 🔹 CARD TOKENS
INSERT INTO CardToken (
  TokenValue, CardBrand, Last4Digits, Expiration,
  TenantId, OwnerId, IsDefault, LinkedOn
) VALUES
('tok_visa_123', 'Visa', '4242', '2026-12-31', 6, NULL, TRUE, NOW()),   -- John Doe
('tok_mc_456', 'MasterCard', '5454', '2025-10-31', 7, NULL, FALSE, NOW()); -- Michael Johnson

-- 🔹 BANK ACCOUNTS
INSERT INTO BankAccountInfo (BankName, AccountNumberMasked, RoutingNumber, AccountType, CreatedOn)
VALUES 
('Chase', '****1234', '021000021', 'Checking', NOW()),
('Bank of America', '****5678', '026009593', 'Savings', NOW());

-- 🔹 BILLING ADDRESSES
INSERT INTO BillingAddress (StreetLine1, StreetLine2, City, State, PostalCode, Country, AvsResult, IsVerified, CreatedOn)
VALUES 
('123 Main St', '', 'Orlando', 'FL', '32801', 'USA', 'Y', TRUE, NOW()),
('456 Elm St', 'Apt 2B', 'Tampa', 'FL', '33602', 'USA', 'N', FALSE, NOW());

-- 🔹 BILLING ADDRESS HISTORY
INSERT INTO BillingAddressHistory (BillingAddressId, StreetLine1, StreetLine2, City, State, PostalCode, Country, AvsResult, IsVerified, ChangedOn)
VALUES 
(1, '123 Main St', '', 'Orlando', 'FL', '32801', 'USA', 'Y', TRUE, NOW()),
(2, '456 Elm St', 'Apt 2B', 'Tampa', 'FL', '33602', 'USA', 'N', FALSE, NOW());

-- ✅ Preferred payment methods
INSERT INTO PreferredMethod (
    TenantId, OwnerId, MethodType, CardTokenId, BankAccountInfoId, IsDefault, UpdatedOn
) VALUES
(6, NULL, 'Card', 1, NULL, TRUE, NOW()),    -- John Doe
(NULL, 4, 'Bank', NULL, 2, TRUE, NOW());    -- Bob Williams

-- 🔹 Invoices (linked to seeded TenantId values: 3, 5, etc.)
INSERT INTO Invoices (
    invoiceId, CustomerName, Email, ReferenceNumber, amount, duedate,
    propertyid, tenantid, IsPaid, status, notes, invoicetypeid, CreatedBy
)
VALUES
(101, 'John Doe', 'john.doe@example.com', 'REF-101', 1200.00, '2024-12-01', 1, 6, FALSE, 'Pending', 'First rent invoice', 1, 'Web'),
(102, 'Michael Johnson', 'michael.johnson@example.com', 'REF-102', 950.00, '2024-12-15', 2, 7, FALSE, 'Pending', 'Lease renewal', 1, 'Web'),
(103, 'John Doe', 'john.doe@example.com', 'REF-103', 500.00, '2025-01-01', 1, 6, FALSE, 'Pending', 'Late fee notice', 2, 'Web');

-- Adjusting to TenantId = 3 and 5 from your seed
INSERT INTO Payments (
    Amount, PaidOn, ReferenceNumber, InvoiceId, TenantId, OwnerId, PaymentType,
    CardType, Last4Digits, AuthorizationCode,
    CheckNumber, CheckBankName,
    BankAccountNumber, RoutingNumber, TransactionId
)
VALUES
(1200.00, NOW(), 'REF-001', 101, 6, NULL, 'Card', 'Visa', '4242', 'AUTH123', NULL, NULL, NULL, NULL, NULL),
(500.00, NOW(), 'REF-003', 103, 6, NULL, 'Transfer', NULL, NULL, NULL, NULL, NULL, '****1234', '021000021', 'TXN456');

INSERT INTO TenantAnnouncements (Title, Message, PostedBy)
VALUES
-- General Maintenance
('Planned Water Outage', 'Please be advised that water will be shut off for maintenance on Thursday from 10:00 AM to 2:00 PM.', 'Property Manager'),
-- Community Event
('Tenant Appreciation BBQ', 'Join us for a BBQ on Saturday at the central courtyard. Food, drinks, and games provided!', 'Community Team'),
-- Weather Alert
('Hurricane Preparedness Notice', 'Please secure outdoor items and review the emergency plan. Updates will be provided via text and email.', 'Management Office'),
-- Policy Update
('Updated Parking Policy', 'All vehicles must be registered with the office. Towing enforcement begins next Monday.', 'Leasing Office'),
-- Routine Reminder
('Rent Reminder', 'This is a friendly reminder that rent is due by the 5th of each month to avoid late fees.', 'System');

INSERT INTO OwnerAnnouncements (Title, Message, PostedBy)
VALUES
-- Financial Notice
('Q2 Financial Statements Available', 
 'You can now access the Q2 financial reports in the owner dashboard. Please review by July 15.', 
 'Finance Team'),
-- Maintenance Update
('Roofing Contractor Appointment', 
 'Roof maintenance for Building C is scheduled next Monday. Please allow roof access from 9am to 4pm.', 
 'Operations'),
-- Community Improvement
('New Landscaping Vendor Selected', 
 'We’ve contracted GreenScape Solutions for monthly landscaping. Improvements will begin next week.', 
 'HOA President'),
-- Security Update
('Gate Access System Upgrade', 
 'The front gate access panel will be replaced this Thursday. Expect a 30-minute outage between 11am–12pm.', 
 'Admin'),
-- Annual Meeting
('Annual Owner Meeting Reminder', 
 'Join us via Zoom on August 10th at 6pm for a review of the year and discussion of upcoming initiatives.', 
 'Board Secretary');
 
 -- Seed bank accounts
INSERT INTO BankAccounts (TenantId, StripeBankAccountId, BankName, Last4, AccountType, IsVerified)
VALUES 
(6, 'ba_test_001', 'Chase', '1234', 'checking', TRUE),
(7, 'ba_test_002', 'Wells Fargo', '5678', 'checking', FALSE);

-- Seed ACH authorizations
INSERT INTO ACHAuthorizations (TenantId, IPAddress, Signature)
VALUES 
(6, '192.168.1.10', 'Signed digitally by tenant'),
(7, '192.168.1.11', 'Signed digitally by tenant');

-- Seed payment transactions
INSERT INTO PaymentTransactions (TenantId, Amount, StripePaymentIntentId, Status)
VALUES 
(6, 1200.00, 'pi_test_001', 'succeeded'),
(6, 1200.00, 'pi_test_002', 'pending'),
(7, 950.00, 'pi_test_003', 'failed');