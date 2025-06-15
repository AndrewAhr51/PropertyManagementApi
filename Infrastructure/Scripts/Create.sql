USE [PropertyManagement]
GO
-- Roles and Permissions
CREATE TABLE [dbo].[Roles] (
    [RoleId] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(50) NOT NULL UNIQUE,
    [Description] NVARCHAR(255)
);
GO
CREATE TABLE [dbo].[Permissions] (
    [PermissionId] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(255)
);
GO
CREATE TABLE RolePermissions (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId), -- ✅ Composite Primary Key
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId)
);
GO
-- Users and Tenants
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    
    -- 🔹 Password Reset Fields
    ResetToken NVARCHAR(255) NULL,
    ResetTokenExpiration DATETIMEOFFSET NULL,

    -- 🔹 Multi-Factor Authentication (MFA)
    MfaCode NVARCHAR(6) NULL,
    MfaCodeExpiration DATETIMEOFFSET NULL,
	
	IsMfaEnabled BIT DEFAULT 1,
    IsActive BIT DEFAULT 1
);

-- ✅ Add Foreign Key for RoleId
ALTER TABLE Users ADD CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId);

-- ✅ Add Indexes for Faster Lookups
CREATE INDEX IX_MfaCode ON Users (MfaCode);
GO
CREATE TABLE [dbo].[Tenants] (
    [TenantId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [FirstName] NVARCHAR(100),
    [LastName] NVARCHAR(100),
    [PhoneNumber] NVARCHAR(20),
    [MoveInDate] DATE,
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
);
GO

CREATE TABLE [dbo].[Property](
	[PropertyId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](255) NOT NULL,
	[Address1] [nvarchar](255) NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NOT NULL,
	[PostalCode] [nvarchar](20) NOT NULL,
	[Country] [nvarchar](100) NOT NULL,
	[Bedrooms] [int] NOT NULL,
	[Bathrooms] [int] NOT NULL,
	[SquareFeet] [int] NOT NULL,
	[IsAvailable] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL
PRIMARY KEY CLUSTERED 
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Property] ADD  DEFAULT ((1)) FOR [IsAvailable]
GO


GO
CREATE TABLE [dbo].[PropertyPhotos](
	[PhotoId] [int] IDENTITY(1,1) NOT NULL,
	[PropertyId] [int] NOT NULL,
	[PhotoUrl] [nvarchar](500) NOT NULL,
	[Caption] [nvarchar](255) NULL,
	[UploadedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[PhotoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PropertyPhotos] ADD  DEFAULT (getdate()) FOR [UploadedAt]
GO

ALTER TABLE [dbo].[PropertyPhotos]  WITH CHECK ADD FOREIGN KEY([PropertyId])
REFERENCES [dbo].[Property] ([PropertyId])

GO
CREATE TABLE [dbo].[Pricing] (
    [PriceId] INT PRIMARY KEY IDENTITY(1,1),
    [PropertyId] INT NOT NULL,
    [RentalAmount] DECIMAL(10,2),
    [DepositAmount] DECIMAL(10,2),
    [LeaseTerm] NVARCHAR(50),
    [UtilitiesIncluded] BIT DEFAULT 0,
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId]) ON DELETE CASCADE
);
GO
-- ✅ Create the Owners Table
CREATE TABLE [dbo].[Owners](
	[OwnerId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](50) NOT NULL,
	[Address1] [nvarchar](255) NOT NULL,
	[Address2] [nvarchar](255) NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NOT NULL,
	[PostalCode] [nvarchar](20) NOT NULL,
	[Country] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL
PRIMARY KEY CLUSTERED 
(
	[OwnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


GO
-- ✅ Create the Association Table (PropertyOwners)
CREATE TABLE dbo.PropertyOwners (
    PropertyId INT NOT NULL,
    OwnerId INT NOT NULL,
    OwnershipPercentage DECIMAL(5,2) NOT NULL CONSTRAINT DF_PropertyOwners_OwnershipPercentage DEFAULT 100,

    CONSTRAINT PK_PropertyOwners PRIMARY KEY (PropertyId, OwnerId),
    CONSTRAINT FK_PropertyOwners_Property FOREIGN KEY (PropertyId) REFERENCES dbo.Property(PropertyId) ON DELETE CASCADE,
    CONSTRAINT FK_PropertyOwners_Owner FOREIGN KEY (OwnerId) REFERENCES dbo.Owners(OwnerId) ON DELETE CASCADE
);
GO
-- Transactions and Billing
CREATE TABLE [dbo].[PaymentMethods] (
    [PaymentMethodId] INT PRIMARY KEY IDENTITY(1,1),
    [MethodName] NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(255),
    [IsActive] BIT NOT NULL
);
GO
CREATE TABLE [dbo].[Payments] (
    [PaymentId] INT PRIMARY KEY IDENTITY(1,1),
    [TenantId] INT,
    [PropertyId] INT,
    [Amount] DECIMAL(10,2),
    [PaymentMethodId] INT,
    [TransactionDate] DATETIME DEFAULT GETDATE(),
    [ReferenceNumber] NVARCHAR(100),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants]([TenantId]),
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId]),
    FOREIGN KEY ([PaymentMethodId]) REFERENCES [dbo].[PaymentMethods]([PaymentMethodId])
);
GO
-- Credit Card Info (optional encryption layer required)
CREATE TABLE [dbo].[CreditCardInfo] (
    [CardId] INT PRIMARY KEY IDENTITY(1,1),
    [TenantId] INT NOT NULL,
    [PropertyId] INT NOT NULL,
    [CardHolderName] NVARCHAR(255),
    [CardNumber] VARBINARY(256),
    [ExpirationDate] DATE,
    [CVV] VARBINARY(256),
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants]([TenantId]),
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId])
);
GO
-- Billing Address
CREATE TABLE [dbo].[BillingAddress] (
    [BillingAddressId] INT PRIMARY KEY IDENTITY(1,1),
    [CardId] INT NOT NULL,
    [AddressLine] NVARCHAR(255) NOT NULL,
    [AddressLine2] NVARCHAR(255) NULL,
    [City] NVARCHAR(100) NOT NULL,
    [State] NVARCHAR(100) NOT NULL,
    [PostalCode] NVARCHAR(20) NOT NULL,
    [Country] NVARCHAR(100) NOT NULL,
    FOREIGN KEY ([CardId]) REFERENCES [dbo].[CreditCardInfo]([CardId]) ON DELETE CASCADE
);
GO
-- Special Instructions
CREATE TABLE [dbo].[SpecialInstructions] (
    [InstructionId] INT PRIMARY KEY IDENTITY(1,1),
    [TenantId] INT NULL,
    [PropertyId] INT NULL,
    [PaymentId] INT NULL,
    [InstructionText] NVARCHAR(MAX),
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants]([TenantId]),
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId]),
    FOREIGN KEY ([PaymentId]) REFERENCES [dbo].[Payments]([PaymentId])
);
GO
CREATE TABLE [dbo].[Emails] (
    [EmailId] INT PRIMARY KEY IDENTITY(1,1),  -- ✅ Unique Identifier
    [SenderId] INT NOT NULL,  -- ✅ FK to Users
    [Recipient] NVARCHAR(255) NOT NULL,  -- ✅ Email Address
    [Subject] NVARCHAR(255) NOT NULL,  -- ✅ Email Subject
    [Body] NVARCHAR(MAX) NOT NULL,  -- ✅ Email Content
    [SentDate] DATETIME DEFAULT GETUTCDATE(),  -- ✅ Auto-populate timestamp
    [Status] NVARCHAR(50) DEFAULT 'Pending',  -- ✅ Tracks email status ('Pending', 'Sent', 'Failed')
    [IsDelivered] BIT DEFAULT 0,  -- ✅ Tracks delivery status

    -- Foreign Keys
    FOREIGN KEY ([SenderId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
);

CREATE TABLE [dbo].[MaintenanceRequests] (
    [RequestId] INT PRIMARY KEY IDENTITY(1,1),
    [TenantId] INT NOT NULL,           -- Who submitted the request
    [PropertyId] INT NOT NULL,         -- Where the issue is
    [RequestDate] DATETIME DEFAULT GETDATE(),  -- When it was submitted
    [Category] NVARCHAR(100),          -- e.g., Plumbing, Electrical, HVAC
    [Description] NVARCHAR(MAX) NOT NULL,  -- Detailed issue report
    [PriorityLevel] NVARCHAR(50) DEFAULT 'Normal',  -- Low, Normal, High, Emergency
    [Status] NVARCHAR(50) DEFAULT 'Open',  -- Open, In Progress, Completed, Cancelled
    [AssignedTo] NVARCHAR(100) NULL,   -- Optional: name of assigned technician
    [ResolutionNotes] NVARCHAR(MAX) NULL,  -- Technician feedback
    [ResolvedDate] DATETIME NULL,     -- When the issue was closed
    [LastUpdated] DATETIME DEFAULT GETDATE(),

    -- Foreign Keys
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants]([TenantId]) ON DELETE CASCADE,
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId]) ON DELETE CASCADE
);
GO
CREATE TABLE [dbo].[Vendors] (
    [VendorId] INT PRIMARY KEY IDENTITY(1,1),
    [Name] NVARCHAR(255) NOT NULL,
    [ServiceType] NVARCHAR(100),
    [ContactEmail] NVARCHAR(255),
    [PhoneNumber] NVARCHAR(20)
);
GO
CREATE TABLE [dbo].[AccessLogs] (
    [LogId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [Action] NVARCHAR(255),
    [Timestamp] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
);
GO

CREATE TABLE [dbo].[Leases] (
    [LeaseId] INT PRIMARY KEY IDENTITY(1,1),
    [TenantId] INT NOT NULL,
    [PropertyId] INT NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NULL,
    [MonthlyRent] DECIMAL(10,2) NOT NULL,
    [DepositPaid] BIT DEFAULT 0,
    [IsActive] BIT DEFAULT 1,
    [SignedDate] DATE DEFAULT GETDATE(),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants]([TenantId]),
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId])
);
GO
CREATE TABLE [dbo].[Invoices] (
    [InvoiceId] INT PRIMARY KEY IDENTITY(1,1),
    [TenantId] INT NOT NULL,
    [PropertyId] INT NOT NULL,
    [AmountDue] DECIMAL(10,2) NOT NULL,
    [DueDate] DATE NOT NULL,
    [IsPaid] BIT DEFAULT 0,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants]([TenantId]),
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId])
);
GO
CREATE TABLE [dbo].[Notes] (
    [NoteId] INT PRIMARY KEY IDENTITY(1,1),
    [CreatedBy] INT NOT NULL,  -- FK to Users
    [TenantId] INT NULL,
    [PropertyId] INT NULL,
    [NoteText] NVARCHAR(MAX) NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([UserId]),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants]([TenantId]),
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId])
);
GO
CREATE TABLE [dbo].[Documents] (
    [DocumentId] INT PRIMARY KEY IDENTITY(1,1),
    [PropertyId] INT NULL,
    [TenantId] INT NULL,
    [FileName] NVARCHAR(255),
    [FileUrl] NVARCHAR(500),
    [Category] NVARCHAR(100), -- Lease, ID, Receipt, etc.
    [UploadedAt] DATETIME DEFAULT GETDATE(),
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId]),
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants]([TenantId])
);
GO
CREATE TABLE [dbo].[PaymentReminders] (
    [ReminderId] INT PRIMARY KEY IDENTITY(1,1),
    [TenantId] INT NOT NULL,
    [PropertyId] INT NOT NULL,
    [InvoiceId] INT NOT NULL,
    [ReminderDate] DATETIME NOT NULL,  -- When the reminder should be sent
    [Status] NVARCHAR(50) DEFAULT 'Pending', -- Pending, Sent, Failed
    FOREIGN KEY ([TenantId]) REFERENCES [dbo].[Tenants]([TenantId]),
    FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property]([PropertyId]),
    FOREIGN KEY ([InvoiceId]) REFERENCES [dbo].[Invoices]([InvoiceId])
);
GO
CREATE INDEX IX_ResetToken ON Users (ResetToken);
CREATE INDEX IX_UserEmail ON Users (Email);
CREATE INDEX IX_Property_City ON Property(City);
CREATE INDEX IX_Property_State ON Property(State);
CREATE INDEX IX_Property_IsAvailable ON Property(IsAvailable);
CREATE INDEX IX_Owners_Email ON Owners(Email);
CREATE INDEX IX_PropertyOwners_PropertyId ON PropertyOwners(PropertyId);
CREATE INDEX IX_PropertyOwners_OwnerId ON PropertyOwners(OwnerId);