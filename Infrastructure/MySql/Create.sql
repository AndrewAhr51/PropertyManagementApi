USE propertymanagement;

CREATE TABLE lkupCategory (
    CategoryId INT PRIMARY KEY AUTO_INCREMENT,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE lkupCreditCards (
    CreditCardID INT PRIMARY KEY AUTO_INCREMENT,
    CreditCardName VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE lkupMaintenanceRequestTypes (
    RequestTypeID INT PRIMARY KEY AUTO_INCREMENT,
    RequestTypeName VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL
);

CREATE TABLE lkupPropertyRooms (
    RoomID INT PRIMARY KEY AUTO_INCREMENT,
    RoomName VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(255) NULL
);

CREATE TABLE lkupServiceTypes (
    ServiceTypeId INT PRIMARY KEY AUTO_INCREMENT,
    TypeName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE lkupInvoiceType (
    InvoiceTypeId INT PRIMARY KEY AUTO_INCREMENT,
    InvoiceType NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Roles Table
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY AUTO_INCREMENT,
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Permissions Table
CREATE TABLE Permissions (
    PermissionId INT PRIMARY KEY AUTO_INCREMENT,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,

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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- PropertyPhotos Table
CREATE TABLE PropertyPhotos (
    PhotoId INT PRIMARY KEY AUTO_INCREMENT,
    PropertyId INT NOT NULL,
    PhotoUrl NVARCHAR(500) NOT NULL,
    Room NVARCHAR(500) NOT NULL,
    Caption NVARCHAR(255) NULL,
    UploadedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
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

-- Payment Methods Lookup
CREATE TABLE lkupPaymentMethods (
    PaymentMethodId INT PRIMARY KEY AUTO_INCREMENT,
    MethodName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CardId) REFERENCES CreditCardInfo(CardId)
);

-- Special Instructions
CREATE TABLE SpecialInstructions (
    InstructionId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT NULL,
    PropertyId INT NULL,
    PaymentId INT NULL,
    InstructionText TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    LastUpdated DATETIME DEFAULT CURRENT_TIMESTAMP,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
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
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
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
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NULL,
    MonthlyRent DECIMAL(10,2) NOT NULL,
    DepositPaid BOOLEAN DEFAULT FALSE,
    IsActive BOOLEAN DEFAULT TRUE,
    SignedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

-- Invoices
CREATE TABLE Invoices (
    InvoiceId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT NOT NULL,
    PropertyId INT NOT NULL,
    AmountDue DECIMAL(10,2) NOT NULL,
    DueDate DATE NOT NULL,
    BillingPeriod NVARCHAR(20) DEFAULT 'Monthly',
    BillingMonth NVARCHAR(10),  
    LateFee DECIMAL(10,2) DEFAULT 0,
    DiscountsApplied DECIMAL(10,2) DEFAULT 0,
    IsPaid BOOLEAN DEFAULT FALSE,
    PaymentDate DATETIME NULL,
    PaymentMethod NVARCHAR(50) NULL,
    PaymentReference NVARCHAR(100) NULL,
    InvoiceStatus NVARCHAR(20) DEFAULT 'Pending',
    InvoiceType NVARCHAR(50) DEFAULT 'Rent',
    GeneratedBy NVARCHAR(50) DEFAULT 'Web',
    Notes NVARCHAR(500) NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId),
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);

-- Notes
CREATE TABLE Notes (
    NoteId INT PRIMARY KEY AUTO_INCREMENT,
    CreatedBy INT NOT NULL,
    TenantId INT NULL,
    PropertyId INT NULL,
    NoteText TEXT NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId)
);

-- Documents
CREATE TABLE Documents (
    DocumentId INT PRIMARY KEY AUTO_INCREMENT,
    PropertyId INT NULL,
    TenantId INT NULL,
    FileName NVARCHAR(255),
    FileUrl NVARCHAR(500),
    Category NVARCHAR(100),
    UploadedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId),
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);

-- Payment Reminders
CREATE TABLE PaymentReminders (
    ReminderId INT PRIMARY KEY AUTO_INCREMENT,
    TenantId INT NOT NULL,
    PropertyId INT NOT NULL,
    InvoiceId INT NOT NULL,
    ReminderDate DATETIME NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Property(PropertyId),
    FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId)
);

-- Optimized Indexes
CREATE INDEX IX_ResetToken ON Users (ResetToken);
CREATE INDEX IX_UserEmail ON Users (Email);
CREATE INDEX IX_Property_City ON Property(City);
CREATE INDEX IX_Property_State ON Property(State);
CREATE INDEX IX_Property_IsAvailable ON Property(IsAvailable);
CREATE INDEX IX_Owners_Email ON Owners(Email);
CREATE INDEX IX_PropertyOwners_PropertyId ON PropertyOwners(PropertyId);
CREATE INDEX IX_PropertyOwners_OwnerId ON PropertyOwners(OwnerId);