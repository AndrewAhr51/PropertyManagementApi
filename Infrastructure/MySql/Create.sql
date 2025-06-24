USE propertymanagement;

CREATE TABLE LkupUtilities (
    UtilityId INT PRIMARY KEY AUTO_INCREMENT,
    UtilityName VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
	IsActive BOOLEAN DEFAULT TRUE,
    CreatedBy Char(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE LkupCleaningType (
    CleaningTypeId INT PRIMARY KEY AUTO_INCREMENT,
    CleaningTypeName VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
	CreatedBy Char(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
);

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
    PropertyTaxes DECIMAL(10,2) default 0,
    Insurance DECIMAL(10,2) default 0,
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
    UploadedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)    
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
    Category NVARCHAR(100) NOT NULL,
    Description TEXT NOT NULL,
    PriorityLevel NVARCHAR(50) DEFAULT 'Low',
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
    PhoneNumber NVARCHAR(20) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    Address1 NVARCHAR(255) NULL,
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(50) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    AccountNumber NVARCHAR(50) UNIQUE NOT NULL,
    Notes TEXT NULL,
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
    DepositAmount DECIMAL(10,2) NOT NULL,
    DepositPaid BOOLEAN DEFAULT FALSE,
    IsActive BOOLEAN DEFAULT TRUE,
    SignedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

CREATE TABLE Invoices (
    invoiceId INT PRIMARY KEY AUTO_INCREMENT,
    CustomerName VARCHAR(100) DEFAULT 'unknown',
    amount DECIMAL(18,2) NOT NULL,
    duedate DATETIME NOT NULL,
    propertyid int NOT NULL,
    IsPaid BOOLEAN DEFAULT FALSE,
    status VARCHAR(50) DEFAULT 'Pending',
    notes TEXT,
    invoicetypeid INT NOT NULL,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Subtype Tables with ON DELETE CASCADE

CREATE TABLE RentInvoices (
    invoiceId INT PRIMARY KEY,
    rentmonth INT,
    rentyear INT,
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);

CREATE TABLE UtilityInvoices (
    invoiceId INT PRIMARY KEY,
    utilitytypeid INT,
    usageamount DECIMAL(10,2),
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);

CREATE TABLE SecurityDepositInvoices (
    invoiceId INT PRIMARY KEY,
    isrefundable BOOLEAN,
	depositamount DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);

CREATE TABLE CleaningFeeInvoices (
    invoiceId INT PRIMARY KEY,
    cleaningtypeid INT,
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);

CREATE TABLE LeaseTerminationInvoices (
    invoiceId INT PRIMARY KEY,
    terminationreason TEXT,
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);

CREATE TABLE ParkingFeeInvoices (
    invoiceId INT PRIMARY KEY,
    spotidentifier VARCHAR(50),
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);

CREATE TABLE PropertyTaxInvoices (
    invoiceId INT PRIMARY KEY,
    taxperiodstart DATE,
    taxperiodend DATE,
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);

CREATE TABLE InsuranceInvoices (
    invoiceId INT PRIMARY KEY,
    policynumber VARCHAR(100),
    coverageperiodstart DATE,
    coverageperiodend DATE,
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);

CREATE TABLE LegalFeeInvoices (
    invoiceId INT PRIMARY KEY,
    casereference VARCHAR(100),
    LawFirm VARCHAR(100),
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
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
    invoiceId INT NOT NULL,    -- Assuming UUID format to match Invoices table
    FileName VARCHAR(255) NOT NULL,
    FileType VARCHAR(50) NOT NULL,   -- PDF, DOCX, JPG, etc.
    FileData LONGBLOB NOT NULL,      -- MySQL equivalent of VARBINARY(MAX)
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (DocumentId) REFERENCES Documents(DocumentId) ON DELETE CASCADE,
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);
-- Payment Reminders
CREATE TABLE PaymentReminders (
    ReminderId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT NOT NULL,
    PropertyId INT NOT NULL,
    invoiceId INT NOT NULL,
    ReminderDate DATETIME NOT NULL,
    Status VARCHAR(50) DEFAULT 'Pending',
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId),
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId)
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

CREATE TABLE CleaningInvoices (
    invoiceId INT PRIMARY KEY,
    -- any specific cleaning invoice fields (e.g., scheduledDate, vendorId, etc.)
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId) ON DELETE CASCADE
);

CREATE TABLE CleaningInvoiceCleaningTypes (
    invoiceId INT NOT NULL,
    cleaningTypeId INT NOT NULL,
    PRIMARY KEY (invoiceId, cleaningTypeId),
    FOREIGN KEY (invoiceId) REFERENCES CleaningInvoices(invoiceId) ON DELETE CASCADE
);

-- Flattened View
CREATE OR REPLACE VIEW InvoiceDetailsView AS
SELECT 
    i.invoiceId,
    i.amount,
    i.duedate,
    i.propertyid,
    i.status,
    i.notes,
    i.invoicetypeid,
    r.rentmonth,
    r.rentyear,
    u.utilitytypeid,
    u.usageamount,
    sd.isrefundable,
    c.cleaningtypeId,
    lt.terminationreason,
    p.spotidentifier,
    pt.taxperiodstart,
    pt.taxperiodend,
    ins.policynumber,
    ins.coverageperiodstart,
    ins.coverageperiodend,
    l.casereference

FROM Invoices i
LEFT JOIN RentInvoices r ON i.invoiceId = r.invoiceId
LEFT JOIN UtilityInvoices u ON i.invoiceId = u.invoiceId
LEFT JOIN SecurityDepositInvoices sd ON i.invoiceId = sd.invoiceId
LEFT JOIN CleaningFeeInvoices c ON i.invoiceId = c.invoiceId
LEFT JOIN LeaseTerminationInvoices lt ON i.invoiceId = lt.invoiceId
LEFT JOIN ParkingFeeInvoices p ON i.invoiceId = p.invoiceId
LEFT JOIN PropertyTaxInvoices pt ON i.invoiceId = pt.invoiceId
LEFT JOIN InsuranceInvoices ins ON i.invoiceId = ins.invoiceId
LEFT JOIN LegalFeeInvoices l ON i.invoiceId = l.invoiceId;

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
CREATE INDEX idx_utility_type ON UtilityInvoices(utilitytypeid);
CREATE INDEX idx_security_refundable ON SecurityDepositInvoices(isrefundable);
CREATE INDEX idx_cleaning_type ON CleaningFeeInvoices(cleaningtypeid);
CREATE INDEX idx_parking_spot ON ParkingFeeInvoices(spotidentifier);
CREATE INDEX idx_property_tax_period ON PropertyTaxInvoices(taxperiodstart, taxperiodend);
CREATE INDEX idx_insurance_policy ON InsuranceInvoices(policynumber);
CREATE INDEX idx_legal_case ON LegalFeeInvoices(casereference);
CREATE INDEX idx_utilityname ON LkupUtilities (UtilityName);
CREATE UNIQUE INDEX idx_utilityname_unique ON LkupUtilities (UtilityName);
CREATE INDEX idx_cleaning_type_name ON LkupCleaningType (CleaningTypeName);
CREATE UNIQUE INDEX idx_cleaning_type_name_unique ON LkupCleaningType (CleaningTypeName);
