USE propertymanagement;

CREATE TABLE lkupPaymentMethods (
    PaymentMethodId INT PRIMARY KEY AUTO_INCREMENT,
    MethodName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedBy Char(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE lkupCategory (
    CategoryId INT PRIMARY KEY AUTO_INCREMENT,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    CreatedBy Char(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE lkupCreditCards (
    CreditCardID INT PRIMARY KEY AUTO_INCREMENT,
    CreditCardName VARCHAR(50) NOT NULL UNIQUE,
    CreatedBy Char(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
    
);

CREATE TABLE lkupMaintenanceRequestTypes (
    RequestTypeID INT PRIMARY KEY AUTO_INCREMENT,
    RequestTypeName VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL
);

CREATE TABLE lkupPropertyRooms (
    RoomID INT PRIMARY KEY AUTO_INCREMENT,
    RoomName VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE lkupServiceTypes (
    ServiceTypeId INT PRIMARY KEY AUTO_INCREMENT,
    TypeName NVARCHAR(100) NOT NULL UNIQUE,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE lkupInvoiceType (
    InvoiceTypeId INT PRIMARY KEY AUTO_INCREMENT,
    InvoiceType NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE lkupInvoiceStatus (
    Id INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Roles Table
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY AUTO_INCREMENT,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Permissions Table
CREATE TABLE Permissions (
    PermissionId INT PRIMARY KEY AUTO_INCREMENT,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Role-Permissions Many-to-Many Mapping
CREATE TABLE RolePermissions (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId)
);

-- Users Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY AUTO_INCREMENT,
    UserName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    -- Password Reset Fields
    ResetToken NVARCHAR(255) NULL,
    ResetTokenExpiration DATETIME NULL,

    -- Multi-Factor Authentication (MFA)
    MfaCode NVARCHAR(6) NULL,
    MfaCodeExpiration DATETIME NULL,
    
    IsMfaEnabled BOOLEAN DEFAULT TRUE,
    IsActive BOOLEAN DEFAULT TRUE
);

-- Foreign Key for RoleId
ALTER TABLE Users ADD CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId);

-- Index for MFA Code
CREATE INDEX IX_MfaCode ON Users (MfaCode);

-- Property Table
CREATE TABLE Property (
    PropertyId INT PRIMARY KEY AUTO_INCREMENT,
    Name NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    Address1 NVARCHAR(255) NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    Bedrooms INT NOT NULL,
    Bathrooms INT NOT NULL,
    SquareFeet INT NOT NULL,
    IsAvailable BOOLEAN DEFAULT TRUE,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- PropertyPhotos Table
CREATE TABLE PropertyPhotos (
    PhotoId INT PRIMARY KEY AUTO_INCREMENT,
    PropertyId INT NOT NULL,
    PhotoUrl NVARCHAR(500) NOT NULL,
    Room NVARCHAR(500) NOT NULL,
    Caption NVARCHAR(255) NULL,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

-- Tenants Table
CREATE TABLE Tenants (
    TenantId INT PRIMARY KEY AUTO_INCREMENT,
    PropertyId INT NOT NULL,
    UserId INT NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    MoveInDate DATE,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

-- Pricing Table
CREATE TABLE Pricing (
    PriceId INT PRIMARY KEY AUTO_INCREMENT,
    PropertyId INT NOT NULL,
    EffectiveDate DATE NOT NULL,
    RentalAmount DECIMAL(10,2),
    DepositAmount DECIMAL(10,2),
    LeaseTerm NVARCHAR(50),
    UtilitiesIncluded BOOLEAN DEFAULT FALSE,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

-- Owners Table
CREATE TABLE Owners (
    OwnerId INT PRIMARY KEY AUTO_INCREMENT,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    Phone NVARCHAR(50) NOT NULL,
    Address1 NVARCHAR(255) NOT NULL,
    Address2 NVARCHAR(255) NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- PropertyOwners (Association Table)
CREATE TABLE PropertyOwners (
    PropertyId INT NOT NULL,
    OwnerId INT NOT NULL,
    OwnershipPercentage DECIMAL(5,2) DEFAULT 100,
    PRIMARY KEY (PropertyId, OwnerId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId),
    FOREIGN KEY (OwnerId) REFERENCES Owners(OwnerId)
);



-- Payments Table
CREATE TABLE Payments (
    PaymentId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT,
    PropertyId INT,
    Amount DECIMAL(10,2),
    PaymentMethodId INT,
    TransactionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ReferenceNumber NVARCHAR(100),
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId),
    FOREIGN KEY (PaymentMethodId) REFERENCES lkupPaymentMethods(PaymentMethodId)
);

-- Credit Card Information
CREATE TABLE CreditCardInfo (
    CardId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT NOT NULL,
    PropertyId INT NOT NULL,
    CardHolderName NVARCHAR(255),
    CardNumber VARBINARY(256),  -- Should be encrypted using MySQL AES functions
    LastFourDigits NVARCHAR(4),
    ExpirationDate NVARCHAR(7),
    CVV VARBINARY(256),  -- Consider security best practices for storage
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

-- Billing Address
CREATE TABLE BillingAddress (
    BillingAddressId INT PRIMARY KEY AUTO_INCREMENT,
    CardId INT NOT NULL,
    AddressLine NVARCHAR(255) NOT NULL,
    AddressLine2 NVARCHAR(255) NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CardId) REFERENCES CreditCardInfo(CardId)
);

-- Special Instructions
CREATE TABLE SpecialInstructions (
    InstructionId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT NULL,
    PropertyId INT NULL,
    PaymentId INT NULL,
    InstructionText TEXT,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId),
    FOREIGN KEY (PaymentId) REFERENCES Payments(PaymentId)
);

-- Emails
CREATE TABLE Emails (
    EmailId INT PRIMARY KEY AUTO_INCREMENT,
    SenderId INT NOT NULL,
    Recipient NVARCHAR(255) NOT NULL,
    Subject NVARCHAR(255) NOT NULL,
    Body TEXT NOT NULL,
    SentDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Status NVARCHAR(50) DEFAULT 'Pending',
    IsDelivered BOOLEAN DEFAULT FALSE,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (SenderId) REFERENCES Users(UserId)
);

-- Maintenance Requests
CREATE TABLE MaintenanceRequests (
    RequestId INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    PropertyId INT NOT NULL,
    RequestDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Category NVARCHAR(100),
    Description TEXT NOT NULL,
    PriorityLevel NVARCHAR(50) DEFAULT 'Normal',
    Status NVARCHAR(50) DEFAULT 'Open',
    AssignedTo NVARCHAR(100) NULL,
    ResolutionNotes TEXT NULL,
    ResolvedDate DATETIME NULL,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

-- Vendors
CREATE TABLE Vendors (
    VendorId INT PRIMARY KEY AUTO_INCREMENT,
    Name NVARCHAR(255) NOT NULL,
    ContactFirstName NVARCHAR(255) NOT NULL,
    ContactLastName NVARCHAR(255) NOT NULL,
    ServiceTypeId INT,
    ContactEmail NVARCHAR(255) UNIQUE NOT NULL,
    PhoneNumber NVARCHAR(20),
    Address NVARCHAR(255),
    Address1 NVARCHAR(255),
    City NVARCHAR(100),
    State NVARCHAR(50),
    PostalCode NVARCHAR(20),
    AccountNumber NVARCHAR(50) UNIQUE NOT NULL,
    Notes TEXT,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE,
    FOREIGN KEY (ServiceTypeId) REFERENCES lkupServiceTypes(ServiceTypeId)
);

-- Indexes for optimized querying
CREATE INDEX IX_Vendors_ServiceType ON Vendors (ServiceTypeId);
CREATE INDEX IX_Vendors_ContactEmail ON Vendors (ContactEmail);
CREATE INDEX IX_Vendors_AccountNumber ON Vendors (AccountNumber);
CREATE INDEX IX_Vendors_City ON Vendors (City);
CREATE INDEX IX_Vendors_State ON Vendors (State);
CREATE INDEX IX_Vendors_PostalCode ON Vendors (PostalCode);

-- Access Logs
CREATE TABLE AccessLogs (
    LogId INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    Action NVARCHAR(255),
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Leases
CREATE TABLE Leases (
    LeaseId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT NOT NULL,
    PropertyId INT NOT NULL,
    Discount INT NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NULL,
    MonthlyRent DECIMAL(10,2) NOT NULL,
    DepositPaid BOOLEAN DEFAULT FALSE,
    IsActive BOOLEAN DEFAULT TRUE,
    SignedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

CREATE TABLE Invoices (
    invoicesId CHAR(36) PRIMARY KEY,
    amount DECIMAL(18,2) NOT NULL,
    duedate DATETIME NOT NULL,
    propertyid CHAR(36) NOT NULL,
    IsPaid BOOLEAN DEFAULT FALSE,
    status VARCHAR(50) DEFAULT 'Pending',
    notes TEXT,
    invoicetypeid int NOT NULL,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- ==============================
-- Subtype Tables
-- ==============================

CREATE TABLE RentInvoices (
    invoicesId CHAR(36) PRIMARY KEY,
    rentmonth INT,
    rentyear INT,
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

CREATE TABLE UtilityInvoices (
    invoicesId CHAR(36) PRIMARY KEY,
    utility_type VARCHAR(50),
    usage_amount DECIMAL(10,2),
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

CREATE TABLE SecurityDepositInvoices (
    invoicesId CHAR(36) PRIMARY KEY,
    is_refundable BOOLEAN,
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

CREATE TABLE CleaningFeeInvoices (
    invoicesId CHAR(36) PRIMARY KEY,
    cleaning_type VARCHAR(100),
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

CREATE TABLE LeaseTerminationInvoices (
    invoicesId CHAR(36) PRIMARY KEY,
    termination_reason TEXT,
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

CREATE TABLE ParkingFeeInvoices (
    invoicesId CHAR(36) PRIMARY KEY,
    spot_identifier VARCHAR(50),
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

CREATE TABLE PropertyTaxInvoices (
    invoicesId CHAR(36) PRIMARY KEY,
    tax_period_start DATE,
    tax_period_end DATE,
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

CREATE TABLE InsuranceInvoices (
    invoicesId CHAR(36) PRIMARY KEY,
    policy_number VARCHAR(100),
    coverage_period_start DATE,
    coverage_period_end DATE,
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

CREATE TABLE LegalFeeInvoices (
    invoicesId CHAR(36) PRIMARY KEY,
    case_reference VARCHAR(100),
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

-- Documents
CREATE TABLE Documents (
    DocumentId INT PRIMARY KEY AUTO_INCREMENT,
    PropertyId INT NULL,
    TenantId INT NULL,
    FileName VARCHAR(255),
    FileUrl VARCHAR(500),
    Category VARCHAR(100),
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId),
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);

CREATE TABLE DocumentStorage (
    DocumentStorageId INT AUTO_INCREMENT PRIMARY KEY,
    DocumentId INT NOT NULL,         -- Foreign key reference to Documents
    invoicesId CHAR(36) NOT NULL,    -- Assuming UUID format to match Invoices table
    FileName VARCHAR(255) NOT NULL,
    FileType VARCHAR(50) NOT NULL,   -- PDF, DOCX, JPG, etc.
    FileData LONGBLOB NOT NULL,      -- MySQL equivalent of VARBINARY(MAX)
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (DocumentId) REFERENCES Documents(DocumentId) ON DELETE CASCADE,
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId) ON DELETE CASCADE
);
-- Payment Reminders
CREATE TABLE PaymentReminders (
    ReminderId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT NOT NULL,
    PropertyId INT NOT NULL,
    invoicesId CHAR(36) NOT NULL,
    ReminderDate DATETIME NOT NULL,
    Status VARCHAR(50) DEFAULT 'Pending',
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId),
    FOREIGN KEY (invoicesId) REFERENCES Invoices(invoicesId)
);

-- Notes
CREATE TABLE Notes (
    NoteId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT NOT NULL,
    PropertyId INT NOT NULL,
    NoteText TEXT NOT NULL,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

-- Flattened View
CREATE OR REPLACE VIEW InvoiceDetailsView AS
SELECT 
    i.invoicesId,
    i.amount,
    i.duedate,
    i.propertyid,
    i.status,
    i.notes,
    i.invoicetypeid,
    r.rentmonth,
    r.rentyear,
    u.utility_type,
    u.usage_amount,
    sd.is_refundable,
    c.cleaning_type,
    lt.termination_reason,
    p.spot_identifier,
    pt.tax_period_start,
    pt.tax_period_end,
    ins.policy_number,
    ins.coverage_period_start,
    ins.coverage_period_end,
    l.case_reference

FROM Invoices i
LEFT JOIN RentInvoices r ON i.invoicesId = r.invoicesId
LEFT JOIN UtilityInvoices u ON i.invoicesId = u.invoicesId
LEFT JOIN SecurityDepositInvoices sd ON i.invoicesId = sd.invoicesId
LEFT JOIN CleaningFeeInvoices c ON i.invoicesId = c.invoicesId
LEFT JOIN LeaseTerminationInvoices lt ON i.invoicesId = lt.invoicesId
LEFT JOIN ParkingFeeInvoices p ON i.invoicesId = p.invoicesId
LEFT JOIN PropertyTaxInvoices pt ON i.invoicesId = pt.invoicesId
LEFT JOIN InsuranceInvoices ins ON i.invoicesId = ins.invoicesId
LEFT JOIN LegalFeeInvoices l ON i.invoicesId = l.invoicesId;

-- Optimized Indexes
CREATE INDEX IX_ResetToken ON Users (ResetToken);
CREATE INDEX IX_UserEmail ON Users (Email);
CREATE INDEX IX_Property_City ON Property(City);
CREATE INDEX IX_Property_State ON Property(State);
CREATE INDEX IX_Property_IsAvailable ON Property(IsAvailable);
CREATE INDEX IX_Owners_Email ON Owners(Email);
CREATE INDEX IX_PropertyOwners_PropertyId ON PropertyOwners(PropertyId);
CREATE INDEX IX_PropertyOwners_OwnerId ON PropertyOwners(OwnerId);
-- Indexes on Invoices
CREATE INDEX idx_invoices_due_date ON Invoices(duedate);
CREATE INDEX idx_invoices_property_id ON Invoices(propertyid);
CREATE INDEX idx_invoices_invoice_type ON Invoices(invoicetypeid);

-- Subtype indexes
CREATE INDEX idx_rent_period ON RentInvoices(rentyear, rentmonth);
CREATE INDEX idx_utility_type ON UtilityInvoices(utility_type);
CREATE INDEX idx_security_refundable ON SecurityDepositInvoices(is_refundable);
CREATE INDEX idx_cleaning_type ON CleaningFeeInvoices(cleaning_type);
CREATE INDEX idx_parking_spot ON ParkingFeeInvoices(spot_identifier);
CREATE INDEX idx_property_tax_period ON PropertyTaxInvoices(tax_period_start, tax_period_end);
CREATE INDEX idx_insurance_policy ON InsuranceInvoices(policy_number);
CREATE INDEX idx_legal_case ON LegalFeeInvoices(case_reference);