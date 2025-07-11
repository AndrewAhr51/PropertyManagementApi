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

CREATE TABLE lkupLineItemType (
    LineItemTypeId INT PRIMARY KEY AUTO_INCREMENT,
    LineItemTypeName NVARCHAR(50) NOT NULL UNIQUE,
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

-- Property Table
CREATE TABLE Properties (
    PropertyId INT AUTO_INCREMENT PRIMARY KEY,
    PropertyName NVARCHAR(255) NOT NULL,
    Address NVARCHAR(255) NOT NULL,
    Address1 NVARCHAR(255),
    City NVARCHAR(100) NOT NULL,
    State NVARCHAR(100) NOT NULL,
    PostalCode NVARCHAR(20) NOT NULL,
    Country NVARCHAR(100) NOT NULL,
    Bedrooms INT NOT NULL,
    Bathrooms INT NOT NULL,
    SquareFeet INT NOT NULL,
    PropertyTaxes DECIMAL(10,2) DEFAULT 0.00,
    Insurance DECIMAL(10,2) DEFAULT 0.00,
    IsAvailable BOOLEAN DEFAULT TRUE,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

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
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId)
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
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId)
);
-- Users Table
CREATE TABLE Users (
    UserId INT PRIMARY KEY AUTO_INCREMENT,
    UserName NVARCHAR(50) NOT NULL,
	Email NVARCHAR(255) NOT NULL UNIQUE,
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

-- Owners Table
CREATE TABLE Owners (
    OwnerId INT PRIMARY KEY,
    PrimaryOwner BOOLEAN DEFAULT FALSE,
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
	Balance DECIMAL(10,2) DEFAULT 0,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (OwnerId) REFERENCES Users(UserId)
);

-- PropertyOwners (Association Table)
CREATE TABLE PropertyOwners (
    PropertyId INT NOT NULL,
    OwnerId INT NOT NULL,
    OwnershipPercentage DECIMAL(5,2) DEFAULT 100,
    PRIMARY KEY (PropertyId, OwnerId),
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
    FOREIGN KEY (OwnerId) REFERENCES Owners(OwnerId)
);

CREATE TABLE OwnerAnnouncements (
    AnnouncementId INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Message TEXT NOT NULL,
    PostedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    PostedBy VARCHAR(100) DEFAULT 'Web',
    IsActive BOOLEAN DEFAULT TRUE
);
-- Tenants Table
CREATE TABLE Tenants (
    TenantId INT PRIMARY KEY,
    PropertyId INT NOT NULL,
    QuickBooksAccessToken NVARCHAR(100) DEFAULT '',
    QuickBooksRefreshToken NVARCHAR(100) DEFAULT '',
    RealmId NVARCHAR(100) DEFAULT '',
    PrimaryTenant BOOLEAN DEFAULT FALSE,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    IsActive BOOLEAN DEFAULT TRUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    MoveInDate DATE,
    Balance DECIMAL(10,2) DEFAULT 0,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Users(UserId),
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId)
);
CREATE TABLE PropertyTenants (
    PropertyId INT NOT NULL,
    TenantId INT NOT NULL,
    PRIMARY KEY (PropertyId, TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);
CREATE TABLE TenantAnnouncements (
    AnnouncementId INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Message TEXT NOT NULL,
    PostedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    PostedBy VARCHAR(100) DEFAULT 'Web',
    IsActive BOOLEAN DEFAULT TRUE
);
-- Emails
CREATE TABLE Emails (
    EmailId INT PRIMARY KEY AUTO_INCREMENT,
    Sender VARCHAR(255) NOT NULL,
    Recipient VARCHAR(255) NOT NULL, -- Use VARCHAR instead of NVARCHAR in MySQL
    Subject VARCHAR(255) NOT NULL,
    Body TEXT NOT NULL,
    AttachmentBlob LONGBLOB, -- Store the PDF as a blob
    SentDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(50) DEFAULT 'Pending',
    IsDelivered BOOLEAN DEFAULT FALSE,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
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
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId)
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
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId)
);

CREATE TABLE InvoiceDocuments (
    InvoiceId INT PRIMARY KEY AUTO_INCREMENT,
	PropertyId INT NOT NULL,
    PropertyName VARCHAR(50),
    TenantId int NULL,
    OwnerId INT NULL,
    TenantName VARCHAR(100) DEFAULT 'unknown',
    Email VARCHAR(255) NOT NULL ,
    ReferenceNumber VARCHAR(50) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    DueDate DATETIME NOT NULL,
    IsPaid BOOLEAN DEFAULT FALSE,
    Status VARCHAR(50) DEFAULT 'Pending',
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE Invoices (
    InvoiceId INT PRIMARY KEY AUTO_INCREMENT,
    LastMonthDue DECIMAL(18,2) NOT NULL,
    LastMonthPaid DECIMAL(18,2) NOT NULL, 
    RentMonth INT,
    RentYear INT,    
    Notes TEXT,
    CreatedBy VARCHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (InvoiceId) REFERENCES InvoiceDocuments(InvoiceId) ON DELETE CASCADE
);

CREATE TABLE InvoiceType (
    InvoiceTypeId INT AUTO_INCREMENT PRIMARY KEY,
    LineItemTypeName VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE InvoiceLineItems (
    LineItemId INT AUTO_INCREMENT PRIMARY KEY,
    InvoiceId INT NOT NULL,
    LineItemTypeId INT NOT NULL,
    Description TEXT,
    Amount DECIMAL(18,2) NOT NULL,
    SortOrder INT DEFAULT 0,
    IsDeleted BOOLEAN DEFAULT FALSE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId) ON DELETE CASCADE,
    FOREIGN KEY (LineItemTypeId) REFERENCES lkupLineItemType(LineItemTypeId),
    INDEX (InvoiceId),
    INDEX (LineItemTypeId)
);

CREATE TABLE InvoiceLineItemMetadata (
    MetadataId INT AUTO_INCREMENT PRIMARY KEY,
    LineItemId INT NOT NULL,
    MetaKey VARCHAR(100) NOT NULL,
    MetaValue TEXT,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (LineItemId) REFERENCES InvoiceLineItems(LineItemId) ON DELETE CASCADE,
    INDEX (LineItemId)
);

CREATE TABLE InvoiceAuditLog (
    AuditId INT AUTO_INCREMENT PRIMARY KEY,
    InvoiceId INT NULL,
    ActionType ENUM('Created', 'Updated', 'Deleted') NOT NULL,
    ChangedBy VARCHAR(50) NOT NULL,
    ChangeTimestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    OldValues JSON NULL,
    NewValues JSON NULL,
    ChangeReason VARCHAR(255) NULL,
    FOREIGN KEY (InvoiceId) REFERENCES invoicedocuments(InvoiceId) ON DELETE SET NULL,
    INDEX (InvoiceId)
);

CREATE TABLE BankAccounts (
    BankAccountId INT AUTO_INCREMENT PRIMARY KEY,
    TenantId INT NOT NULL,
    StripeBankAccountId VARCHAR(255) NOT NULL,
    BankName VARCHAR(100),
    Last4 VARCHAR(4),
    AccountType ENUM('checking', 'savings') DEFAULT 'checking',
    IsVerified BOOLEAN DEFAULT FALSE,
    AddedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);
-- Payment Table

CREATE TABLE Payments (
    PaymentId INT AUTO_INCREMENT PRIMARY KEY,
    Amount DECIMAL(10,2) NOT NULL,
    PaidOn DATETIME NULL,
    ReferenceNumber VARCHAR(50) NOT NULL,
    InvoiceId INT NOT NULL,
    TenantId INT NULL,  -- ✅ Now nullable
    OwnerId INT NULL,   -- ✅ Now nullable
    PaymentType VARCHAR(20) NOT NULL, -- "Card", "Check", "Transfer"

    -- Card fields
    CardType VARCHAR(20),
    Last4Digits VARCHAR(4),
    AuthorizationCode VARCHAR(50),

    -- Check fields
    CheckNumber VARCHAR(30),
    CheckBankName VARCHAR(50),
    TransactionId VARCHAR(50),

    FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId),
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (OwnerId) REFERENCES Owners(OwnerId)
);

CREATE TABLE PaymentMetadata (
    PaymentMetadataId INT AUTO_INCREMENT PRIMARY KEY,
    PaymentId INT NOT NULL,
    `Key` VARCHAR(100) NOT NULL,
    `Value` TEXT,

    -- Foreign key constraint
    CONSTRAINT fk_payment_metadata_payment
        FOREIGN KEY (PaymentId)
        REFERENCES Payments(PaymentId)
        ON DELETE CASCADE
);
CREATE TABLE ElectronicTransferPayments(
	PaymentId INT PRIMARY KEY,
    BankAccountNumber VARCHAR(30),
    RoutingNumber VARCHAR(20),    
    FOREIGN KEY (PaymentId) REFERENCES Payments(PaymentId) ON DELETE CASCADE
);

CREATE TABLE PaymentAuditLog (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    PaymentId INT NULL,
    Gateway VARCHAR(50) NOT NULL,
    Action VARCHAR(50) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    ResponsePayload JSON,
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    PerformedBy VARCHAR(100),

    FOREIGN KEY (PaymentId) REFERENCES Payments(PaymentId) ON DELETE SET NULL
);
CREATE TABLE CheckPayments (
    PaymentId INT PRIMARY KEY,
    CheckNumber VARCHAR(50),
    CheckBankName VARCHAR(100),
    FOREIGN KEY (PaymentId) REFERENCES Payments(PaymentId) ON DELETE CASCADE
);
  
CREATE TABLE WireTransfers (
    PaymentId INT PRIMARY KEY,
    BankAccountNumber VARCHAR(50),
    RoutingNumber VARCHAR(20),
    TransactionId VARCHAR(50),
    FOREIGN KEY (PaymentId) REFERENCES Payments(PaymentId) ON DELETE CASCADE
);
CREATE TABLE CardPayments (
    PaymentId INT PRIMARY KEY,
    OrderId VARCHAR(100),
    Status VARCHAR(100),
    CardType VARCHAR(50),
    Last4Digits VARCHAR(4),
    AuthorizationCode VARCHAR(50),
    FOREIGN KEY (PaymentId) REFERENCES Payments(PaymentId) ON DELETE CASCADE
);

CREATE TABLE BillingAddress (
    BillingAddressId INT AUTO_INCREMENT PRIMARY KEY,
    StreetLine1 VARCHAR(100),
    StreetLine2 VARCHAR(100),
    City VARCHAR(50),
    State VARCHAR(50),
    PostalCode VARCHAR(20),
    Country VARCHAR(50),
    AvsResult VARCHAR(10),
    IsVerified BOOLEAN DEFAULT FALSE,
    CreatedOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE TABLE BillingAddressHistory (
    BillingAddressHistoryId INT AUTO_INCREMENT PRIMARY KEY,
    BillingAddressId INT NOT NULL,
    StreetLine1 VARCHAR(100),
    StreetLine2 VARCHAR(100),
    City VARCHAR(50),
    State VARCHAR(50),
    PostalCode VARCHAR(20),
    Country VARCHAR(50),
    AvsResult VARCHAR(10),
    IsVerified BOOLEAN DEFAULT FALSE,
    ChangedOn DATETIME NOT NULL,

    FOREIGN KEY (BillingAddressId) REFERENCES BillingAddress(BillingAddressId)
);

CREATE TABLE CardToken (
    CardTokenId INT AUTO_INCREMENT PRIMARY KEY,
    TokenValue VARCHAR(100) NOT NULL,
    CardBrand VARCHAR(20),
    Last4Digits VARCHAR(4),
    Expiration DATE NOT NULL,
    TenantId INT,
    OwnerId INT ,
    IsDefault BOOLEAN DEFAULT FALSE,
    LinkedOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (OwnerId) REFERENCES Owners(OwnerId)
);

CREATE TABLE PreferredMethod (
    PreferredMethodId INT AUTO_INCREMENT PRIMARY KEY,
    TenantId INT,
    OwnerId INT,
    MethodType VARCHAR(20) NOT NULL, -- "Card", "Bank", etc.
    CardTokenId INT,
    BankAccountId INT,
    IsDefault BOOLEAN DEFAULT FALSE,
    UpdatedOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (OwnerId) REFERENCES Owners(OwnerId),
    FOREIGN KEY (CardTokenId) REFERENCES CardToken(CardTokenId),
    FOREIGN KEY (BankAccountId) REFERENCES BankAccounts(BankAccountId)
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
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
    FOREIGN KEY (PaymentId) REFERENCES Payments(PaymentId)
);
-- Documents
CREATE TABLE Documents (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Name VARCHAR(255) NOT NULL,
  MimeType VARCHAR(100) NOT NULL,
  SizeInBytes BIGINT NOT NULL,
  DocumentType VARCHAR(100),
  CreateDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,   
  CreatedBy INT NOT NULL,
  IsEncrypted BOOLEAN DEFAULT FALSE,
  Checksum CHAR(64),  -- Optional SHA-256 hash
  CorrelationId VARCHAR(128),
  Status VARCHAR(50) DEFAULT 'Active',
  Content LONGBLOB NOT NULL,

  INDEX idx_CreatedBy (CreatedByUserId),
  INDEX idx_DocumentType (DocumentType),
  INDEX idx_CreateDate (CreateDate)
);

CREATE TABLE DocumentReferences (
  Id INT AUTO_INCREMENT PRIMARY KEY,
  DocumentId INT NOT NULL,
  RelatedEntityType VARCHAR(50) NOT NULL,     -- 'Tenant', 'Owner', etc.
  RelatedEntityId INT NOT NULL,
  AccessRole VARCHAR(50),                     -- 'Uploader', 'Signer', etc.
  LinkedAtUtc DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

  FOREIGN KEY (DocumentId) REFERENCES Documents(Id) ON DELETE CASCADE,

  INDEX idx_EntityLookup (RelatedEntityType, RelatedEntityId),
  INDEX idx_DocumentAccess (DocumentId, AccessRole)
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
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
    FOREIGN KEY (invoiceId) REFERENCES Invoices(invoiceId)
);

-- Notes

CREATE TABLE ACHAuthorizations (
    ACHAuthorizationId INT AUTO_INCREMENT PRIMARY KEY,
    TenantId INT NOT NULL,
    AuthorizedOn DATETIME DEFAULT CURRENT_TIMESTAMP,
    IPAddress VARCHAR(45),
    Signature TEXT,
    IsRevoked BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);
CREATE TABLE PaymentTransactions (
    PaymentTransactionId INT AUTO_INCREMENT PRIMARY KEY,
    TenantId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    StripePaymentIntentId VARCHAR(255) NOT NULL,
    Status ENUM('pending', 'succeeded', 'failed', 'canceled') DEFAULT 'pending',
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
);

CREATE TABLE QuickBooksAuditLog (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    TenantId INT NOT NULL,
    RealmId VARCHAR(64) NOT NULL,
    EventType VARCHAR(100) NOT NULL,
    CorrelationId VARCHAR(128),
    TimestampUtc DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_Tenant_Timestamp (TenantId, TimestampUtc),
    INDEX idx_Realm (RealmId)
);
CREATE TABLE TriggerLog (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Message VARCHAR(255),
    FiredAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 📘 Start custom delimiter for compound statements
DELIMITER $$

-- 🔄 After INSERT on InvoiceDocuments — Adjust balance
CREATE TRIGGER trg_after_invoice_insert
AFTER INSERT ON InvoiceDocuments
FOR EACH ROW
BEGIN
    IF NEW.IsPaid = FALSE THEN
        UPDATE Tenants
        SET Balance = Balance + NEW.Amount
        WHERE TenantId = NEW.TenantId;
    END IF;
END$$

-- ✏️ After UPDATE on InvoiceDocuments — Adjust balance delta or reverse
CREATE TRIGGER trg_after_invoice_update
AFTER UPDATE ON InvoiceDocuments
FOR EACH ROW
BEGIN
    DECLARE delta DECIMAL(18,2);

    IF OLD.IsPaid = FALSE AND NEW.IsPaid = FALSE AND OLD.Amount != NEW.Amount THEN
        SET delta = NEW.Amount - OLD.Amount;
        UPDATE Tenants
        SET Balance = Balance + delta
        WHERE TenantId = NEW.TenantId;
    END IF;

    IF OLD.IsPaid = FALSE AND NEW.IsPaid = TRUE THEN
        UPDATE Tenants
        SET Balance = Balance - OLD.Amount
        WHERE TenantId = NEW.TenantId;
    END IF;

    IF OLD.IsPaid = TRUE AND NEW.IsPaid = FALSE THEN
        UPDATE Tenants
        SET Balance = Balance + NEW.Amount
        WHERE TenantId = NEW.TenantId;
    END IF;
END$$

-- 🗑️ Before DELETE on InvoiceDocuments — Reverse unpaid balance
CREATE TRIGGER trg_before_invoice_delete
BEFORE DELETE ON InvoiceDocuments
FOR EACH ROW
BEGIN
    IF OLD.IsPaid = FALSE THEN
        UPDATE Tenants
        SET Balance = Balance - OLD.Amount
        WHERE TenantId = OLD.TenantId;
    END IF;
END$$

-- 🧾 Audit After INSERT on invoices
CREATE TRIGGER trg_audit_invoicedocuments_insert
AFTER INSERT ON invoicedocuments
FOR EACH ROW
BEGIN
    INSERT INTO InvoiceAuditLog (
        InvoiceId, ActionType, ChangedBy, NewValues
    ) VALUES (
        NEW.InvoiceId,
        'Created',
        NEW.CreatedBy,
        JSON_OBJECT(
            'PropertyId', NEW.PropertyId,
            'PropertyName', NEW.PropertyName,
            'TenantId', NEW.TenantId,
            'OwnerId', NEW.OwnerId,
            'Amount', NEW.Amount,
            'DueDate', NEW.DueDate
        )
    );
END$$

-- 📘 Audit After UPDATE on invoices
CREATE TRIGGER trg_audit_invoicedocuments_update
AFTER UPDATE ON invoicedocuments
FOR EACH ROW
BEGIN
    INSERT INTO InvoiceAuditLog (
        InvoiceId, ActionType, ChangedBy, OldValues, NewValues
    ) VALUES (
        NEW.InvoiceId,
        'Updated',
        NEW.CreatedBy,
        JSON_OBJECT(
            'PropertyId', OLD.PropertyId,
            'PropertyName', OLD.PropertyName,
            'TenantId', OLD.TenantId,
            'OwnerId', OLD.OwnerId,
            'Amount', OLD.Amount,
            'DueDate', OLD.DueDate
        ),
        JSON_OBJECT(
            'PropertyId', NEW.PropertyId,
            'PropertyName', NEW.PropertyName,
            'TenantId', NEW.TenantId,
            'OwnerId', NEW.OwnerId,
            'Amount', NEW.Amount,
            'DueDate', NEW.DueDate
        )
    );
END$$

-- 🧹 Audit Before DELETE on invoices
CREATE TRIGGER trg_audit_invoicedocuments_delete
BEFORE DELETE ON invoicedocuments
FOR EACH ROW
BEGIN
    INSERT INTO InvoiceAuditLog (
        InvoiceId, ActionType, ChangedBy, OldValues
    ) VALUES (
        OLD.InvoiceId,
        'Deleted',
        OLD.CreatedBy,
        JSON_OBJECT(
            'PropertyId', OLD.PropertyId,
            'PropertyName', OLD.PropertyName,
            'TenantId', OLD.TenantId,
            'OwnerId', OLD.OwnerId,
            'Amount', OLD.Amount,
            'DueDate', OLD.DueDate
        )
    );
END$$

-- 📦 BillingAddress history triggers
CREATE TRIGGER LogBillingAddressInsert
AFTER INSERT ON BillingAddress
FOR EACH ROW
BEGIN
    INSERT INTO BillingAddressHistory (
        BillingAddressId, StreetLine1, StreetLine2, City, State,
        PostalCode, Country, AvsResult, IsVerified, ChangedOn
    )
    VALUES (
        NEW.BillingAddressId, NEW.StreetLine1, NEW.StreetLine2,
        NEW.City, NEW.State, NEW.PostalCode, NEW.Country,
        NEW.AvsResult, NEW.IsVerified, NOW()
    );
END$$

CREATE TRIGGER LogBillingAddressUpdate
AFTER UPDATE ON BillingAddress
FOR EACH ROW
BEGIN
    INSERT INTO BillingAddressHistory (
        BillingAddressId, StreetLine1, StreetLine2, City, State,
        PostalCode, Country, AvsResult, IsVerified, ChangedOn
    )
    VALUES (
        OLD.BillingAddressId, OLD.StreetLine1, OLD.StreetLine2,
        OLD.City, OLD.State, OLD.PostalCode, OLD.Country,
        OLD.AvsResult, OLD.IsVerified, NOW()
    );
END$$

CREATE TRIGGER LogBillingAddressDelete
BEFORE DELETE ON BillingAddress
FOR EACH ROW
BEGIN
    INSERT INTO BillingAddressHistory (
        BillingAddressId, StreetLine1, StreetLine2, City, State,
        PostalCode, Country, AvsResult, IsVerified, ChangedOn
    )
    VALUES (
        OLD.BillingAddressId, OLD.StreetLine1, OLD.StreetLine2,
        OLD.City, OLD.State, OLD.PostalCode, OLD.Country,
        OLD.AvsResult, OLD.IsVerified, NOW()
    );
END$$

-- ✅ Reset the delimiter
DELIMITER ;

-- Optimized Indexes
CREATE INDEX IX_ResetToken ON Users (ResetToken);
CREATE INDEX IX_TenantEmail ON Tenants (Email);
CREATE INDEX IX_OwnerEmail ON Owners (Email);
CREATE INDEX IX_Property_City ON Properties(City);
CREATE INDEX IX_Property_State ON Properties(State);
CREATE INDEX IX_Property_IsAvailable ON Properties(IsAvailable);
CREATE INDEX IX_Owners_Email ON Owners(Email);
CREATE INDEX IX_PropertyOwners_PropertyId ON PropertyOwners(PropertyId);
CREATE INDEX IX_PropertyOwners_OwnerId ON PropertyOwners(OwnerId);
-- Indexes on Invoices
CREATE INDEX idx_invoices_due_date ON InvoiceDocuments(duedate);
CREATE INDEX idx_invoices_property_id ON InvoiceDocuments(propertyid);

-- Subtype indexes
CREATE INDEX idx_utilityname ON LkupUtilities (UtilityName);
CREATE UNIQUE INDEX idx_utilityname_unique ON LkupUtilities (UtilityName);
CREATE UNIQUE INDEX idx_cleaning_type_name_unique ON LkupCleaningType (CleaningTypeName);
