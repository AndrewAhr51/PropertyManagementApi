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

-- Property Table
CREATE TABLE Properties (
    PropertyId INT PRIMARY KEY AUTO_INCREMENT,
    PropertyName NVARCHAR(255) NOT NULL,
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
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId)
);

-- Tenants Table
CREATE TABLE Tenants (
    TenantId INT PRIMARY KEY AUTO_INCREMENT,
    PropertyId INT NOT NULL,
    UserId INT NOT NULL,
    PrimaryTenant BOOLEAN DEFAULT FALSE,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
	Email NVARCHAR(255) NOT NULL UNIQUE,
    MoveInDate DATE,
    Balance DECIMAL(10,2) DEFAULT 0,
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
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

-- Owners Table
CREATE TABLE Owners (
    OwnerId INT PRIMARY KEY AUTO_INCREMENT,
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
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
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

CREATE TABLE PropertyTenants (
    PropertyId INT NOT NULL,
    TenantId INT NOT NULL,
    PRIMARY KEY (PropertyId, TenantId),
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId)
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

CREATE TABLE Invoices (
    invoiceId INT PRIMARY KEY AUTO_INCREMENT,
    CustomerName VARCHAR(100) DEFAULT 'unknown',
    Email VARCHAR(255) NOT NULL ,
    ReferenceNumber VARCHAR(50) NOT NULL, 
    amount DECIMAL(18,2) NOT NULL,
    duedate DATETIME NOT NULL,
    propertyid int NOT NULL,
    tenantid int NULL,
    ownerid int NULL,
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

-- Payment Table
CREATE TABLE Payments (
    PaymentId INT AUTO_INCREMENT PRIMARY KEY,
    Amount DECIMAL(10,2) NOT NULL,
    PaidOn DATETIME NOT NULL,
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

    -- Transfer fields
    BankAccountNumber VARCHAR(30),
    RoutingNumber VARCHAR(20),
    TransactionId VARCHAR(50),

    FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId),
    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (OwnerId) REFERENCES Owners(OwnerId)
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
CREATE TABLE CreditCardPayments (
    PaymentId INT PRIMARY KEY,
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

CREATE TABLE BankAccountInfo (
    BankAccountInfoId INT AUTO_INCREMENT PRIMARY KEY,
    BankName VARCHAR(50),
    AccountNumberMasked VARCHAR(20),
    RoutingNumber VARCHAR(20),
    AccountType VARCHAR(20),
    CreatedOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE CardToken (
    CardTokenId INT AUTO_INCREMENT PRIMARY KEY,
    TokenValue VARCHAR(100) NOT NULL,
    CardBrand VARCHAR(20),
    Last4Digits VARCHAR(4),
    Expiration DATE NOT NULL,
    TenantId INT NOT NULL,
    OwnerId INT NOT NULL,
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
    BankAccountInfoId INT,
    IsDefault BOOLEAN DEFAULT FALSE,
    UpdatedOn DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (TenantId) REFERENCES Tenants(TenantId),
    FOREIGN KEY (OwnerId) REFERENCES Owners(OwnerId),
    FOREIGN KEY (CardTokenId) REFERENCES CardToken(CardTokenId),
    FOREIGN KEY (BankAccountInfoId) REFERENCES BankAccountInfo(BankAccountInfoId)
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
    DocumentId INT PRIMARY KEY AUTO_INCREMENT,
    PropertyId INT NULL,
    TenantId INT NULL,
    FileName VARCHAR(255),
    FileUrl VARCHAR(500),
    Category VARCHAR(100),
    CreatedBy CHAR(50) DEFAULT 'Web',
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
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
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId),
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
    FOREIGN KEY (PropertyId) REFERENCES Properties(PropertyId)
);

CREATE TABLE InvoiceAuditLog (
    AuditId INT AUTO_INCREMENT PRIMARY KEY,
    InvoiceId INT NOT NULL,
    ActionType ENUM('Created', 'Updated', 'Deleted') NOT NULL,
    ChangedBy CHAR(50) NOT NULL,
    ChangeTimestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    OldValues JSON NULL,
    NewValues JSON NULL,
    ChangeReason VARCHAR(255) NULL,

    FOREIGN KEY (InvoiceId) REFERENCES Invoices(InvoiceId)
);

CREATE TABLE TriggerLog (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Message VARCHAR(255),
    FiredAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 📘 Start custom delimiter for compound statements
DELIMITER $$

-- 🔸 1. Update Tenant Balance When Invoice Is Inserted
CREATE TRIGGER after_invoice_insert
AFTER INSERT ON Invoices
FOR EACH ROW
BEGIN
    IF NEW.IsPaid = FALSE THEN
        UPDATE Tenants
        SET Balance = Balance + NEW.Amount
        WHERE TenantId = NEW.TenantId;
    END IF;
END;

-- 🔸 2. Adjust Tenant Balance on Invoice Update
CREATE TRIGGER after_invoice_update
AFTER UPDATE ON Invoices
FOR EACH ROW
BEGIN
    DECLARE delta DECIMAL(10,2);
    -- Amount changed while unpaid
    IF OLD.IsPaid = FALSE AND NEW.IsPaid = FALSE AND OLD.Amount != NEW.Amount THEN
        SET delta = NEW.Amount - OLD.Amount;
        UPDATE Tenants
        SET Balance = Balance + delta
        WHERE TenantId = NEW.TenantId;
    END IF;

    -- Invoice marked as paid
    IF OLD.IsPaid = FALSE AND NEW.IsPaid = TRUE THEN
        UPDATE Tenants
        SET Balance = Balance - OLD.Amount
        WHERE TenantId = NEW.TenantId;
    END IF;

    -- Invoice marked as unpaid again
    IF OLD.IsPaid = TRUE AND NEW.IsPaid = FALSE THEN
        UPDATE Tenants
        SET Balance = Balance + NEW.Amount
        WHERE TenantId = NEW.TenantId;
    END IF;
END;

-- 🔸 3. Subtract Balance if Unpaid Invoice Is Deleted
CREATE TRIGGER before_invoice_delete
BEFORE DELETE ON Invoices
FOR EACH ROW
BEGIN
    IF OLD.IsPaid = FALSE THEN
        UPDATE Tenants
        SET Balance = Balance - OLD.Amount
        WHERE TenantId = OLD.TenantId;
    END IF;
END;

-- 🔸 4. Log Invoice Creation in Audit Table
CREATE TRIGGER audit_invoice_insert
AFTER INSERT ON Invoices
FOR EACH ROW
BEGIN
    INSERT INTO InvoiceAuditLog (InvoiceId, ActionType, ChangedBy, NewValues)
    VALUES (
        NEW.InvoiceId,
        'Created',
        NEW.CreatedBy,
        JSON_OBJECT(
            'Amount', NEW.Amount,
            'DueDate', NEW.DueDate,
            'Status', NEW.Status,
            'IsPaid', NEW.IsPaid
        )
    );
END;

-- 🔸 5. Log Invoice Updates
CREATE TRIGGER audit_invoice_update
AFTER UPDATE ON Invoices
FOR EACH ROW
BEGIN
    INSERT INTO InvoiceAuditLog (InvoiceId, ActionType, ChangedBy, OldValues, NewValues)
    VALUES (
        NEW.InvoiceId,
        'Updated',
        NEW.CreatedBy,
        JSON_OBJECT(
            'Amount', OLD.Amount,
            'DueDate', OLD.DueDate,
            'Status', OLD.Status,
            'IsPaid', OLD.IsPaid
        ),
        JSON_OBJECT(
            'Amount', NEW.Amount,
            'DueDate', NEW.DueDate,
            'Status', NEW.Status,
            'IsPaid', NEW.IsPaid
        )
    );
END;

-- 🔸 6. Log Invoice Deletion
CREATE TRIGGER audit_invoice_delete
BEFORE DELETE ON Invoices
FOR EACH ROW
BEGIN
    INSERT INTO InvoiceAuditLog (InvoiceId, ActionType, ChangedBy, OldValues)
    VALUES (
        OLD.InvoiceId,
        'Deleted',
        OLD.CreatedBy,
        JSON_OBJECT(
            'Amount', OLD.Amount,
            'DueDate', OLD.DueDate,
            'Status', OLD.Status,
            'IsPaid', OLD.IsPaid
        )
    );
END;

CREATE TRIGGER LogBillingAddressUpdate
AFTER UPDATE ON BillingAddress
FOR EACH ROW
BEGIN
    INSERT INTO BillingAddressHistory (
        BillingAddressId,
        StreetLine1,
        StreetLine2,
        City,
        State,
        PostalCode,
        Country,
        AvsResult,
        IsVerified,
        ChangedOn
    )
    VALUES (
        OLD.BillingAddressId,
        OLD.StreetLine1,
        OLD.StreetLine2,
        OLD.City,
        OLD.State,
        OLD.PostalCode,
        OLD.Country,
        OLD.AvsResult,
        OLD.IsVerified,
        NOW()
    );
END$$

CREATE TRIGGER LogBillingAddressInsert
AFTER INSERT ON BillingAddress
FOR EACH ROW
BEGIN
    INSERT INTO BillingAddressHistory (
        BillingAddressId,
        StreetLine1,
        StreetLine2,
        City,
        State,
        PostalCode,
        Country,
        AvsResult,
        IsVerified,
        ChangedOn
    )
    VALUES (
        NEW.BillingAddressId,
        NEW.StreetLine1,
        NEW.StreetLine2,
        NEW.City,
        NEW.State,
        NEW.PostalCode,
        NEW.Country,
        NEW.AvsResult,
        NEW.IsVerified,
        NOW()
    );
END$$

CREATE TRIGGER LogBillingAddressDelete
BEFORE DELETE ON BillingAddress
FOR EACH ROW
BEGIN
    INSERT INTO BillingAddressHistory (
        BillingAddressId,
        StreetLine1,
        StreetLine2,
        City,
        State,
        PostalCode,
        Country,
        AvsResult,
        IsVerified,
        ChangedOn
    )
    VALUES (
        OLD.BillingAddressId,
        OLD.StreetLine1,
        OLD.StreetLine2,
        OLD.City,
        OLD.State,
        OLD.PostalCode,
        OLD.Country,
        OLD.AvsResult,
        OLD.IsVerified,
        NOW()
    );
END$$

CREATE TRIGGER trg_adjust_balance_after_payment
AFTER INSERT ON Payments
FOR EACH ROW
BEGIN
    IF NEW.TenantId IS NOT NULL AND NEW.OwnerId IS NULL THEN
        UPDATE Tenants
        SET Balance = Balance - NEW.Amount
        WHERE TenantId = NEW.TenantId;
		INSERT INTO TriggerLog (Message) VALUES (CONCAT('Trigger fired for TenantId: ', NEW.TenantId));
        
    ELSEIF NEW.OwnerId IS NOT NULL AND NEW.TenantId IS NULL THEN
        UPDATE Owners
        SET Balance = Balance + NEW.Amount
        WHERE OwnerId = NEW.OwnerId;
        
        INSERT INTO TriggerLog (Message) VALUES (CONCAT('Trigger fired for OwnerId: ', NEW.OwnerId));
    END IF;
    
END$$

CREATE TRIGGER trg_adjust_balance_after_payment_update
AFTER UPDATE ON Payments
FOR EACH ROW
BEGIN
    -- Tenant-only payment adjustment
    IF NEW.TenantId IS NOT NULL AND NEW.OwnerId IS NULL THEN
        UPDATE Tenants
        SET Balance = Balance + OLD.Amount - NEW.Amount
        WHERE TenantId = NEW.TenantId;

    -- Owner-only payment adjustment
    ELSEIF NEW.OwnerId IS NOT NULL AND NEW.TenantId IS NULL THEN
        UPDATE Owners
        SET Balance = Balance - OLD.Amount + NEW.Amount
        WHERE OwnerId = NEW.OwnerId;
    END IF;
END$$

CREATE TRIGGER trg_adjust_balance_after_payment_delete
AFTER DELETE ON Payments
FOR EACH ROW
BEGIN
    -- If payment was for a tenant only
    IF OLD.TenantId IS NOT NULL AND OLD.OwnerId IS NULL THEN
        UPDATE Tenants
        SET Balance = Balance + OLD.Amount
        WHERE TenantId = OLD.TenantId;

    -- If payment was for an owner only
    ELSEIF OLD.OwnerId IS NOT NULL AND OLD.TenantId IS NULL THEN
        UPDATE Owners
        SET Balance = Balance - OLD.Amount
        WHERE OwnerId = OLD.OwnerId;
    END IF;
END$$

-- Reset to default delimiter
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
CREATE UNIQUE INDEX idx_cleaning_type_name_unique ON LkupCleaningType (CleaningTypeName);
