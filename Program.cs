using CorrelationId;
using CorrelationId.DependencyInjection;
using Going.Plaid;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PayPalCheckoutSdk.Core;
using PdfSharp.Fonts;
using PropertyManagementAPI.Application.Configuration;
using PropertyManagementAPI.Application.Services.Accounting.Quickbooks;
using PropertyManagementAPI.Application.Services.Auth;
using PropertyManagementAPI.Application.Services.Documents;
using PropertyManagementAPI.Application.Services.Email;
using PropertyManagementAPI.Application.Services.Intuit;
using PropertyManagementAPI.Application.Services.InvoiceExport;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Application.Services.Notes;
using PropertyManagementAPI.Application.Services.OwnerAnnouncements;
using PropertyManagementAPI.Application.Services.Owners;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Application.Services.Payments.CardTokens;
using PropertyManagementAPI.Application.Services.Payments.PayPal;
using PropertyManagementAPI.Application.Services.Payments.Plaid;
using PropertyManagementAPI.Application.Services.Payments.PreferredMethods;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Application.Services.Property;
using PropertyManagementAPI.Application.Services.Quickbooks;
using PropertyManagementAPI.Application.Services.Roles;
using PropertyManagementAPI.Application.Services.TenantAnnouncements;
using PropertyManagementAPI.Application.Services.Tenants;
using PropertyManagementAPI.Application.Services.Users;
using PropertyManagementAPI.Application.Services.Vendors;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Infrastructure.Auditing;
//
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Payments;
using PropertyManagementAPI.Infrastructure.Quickbooks;
using PropertyManagementAPI.Infrastructure.Repositories.Documents;
using PropertyManagementAPI.Infrastructure.Repositories.Email;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Notes;
using PropertyManagementAPI.Infrastructure.Repositories.OwnerAnnouncements;
using PropertyManagementAPI.Infrastructure.Repositories.Owners;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using PropertyManagementAPI.Infrastructure.Repositories.Payments.Banking;
using PropertyManagementAPI.Infrastructure.Repositories.Payments.CardTokens;
using PropertyManagementAPI.Infrastructure.Repositories.Payments.PreferredMethods;
using PropertyManagementAPI.Infrastructure.Repositories.Property;
using PropertyManagementAPI.Infrastructure.Repositories.Quickbooks;
using PropertyManagementAPI.Infrastructure.Repositories.Roles;
using PropertyManagementAPI.Infrastructure.Repositories.TenantAnnouncements;
using PropertyManagementAPI.Infrastructure.Repositories.Tenants;
using PropertyManagementAPI.Infrastructure.Repositories.Users;
using PropertyManagementAPI.Infrastructure.Repositories.Vendors;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Environment = System.Environment;
using PlaidOptions = PropertyManagementAPI.Infrastructure.Payments.PlaidOptions;

var builder = WebApplication.CreateBuilder(args);

GlobalFontSettings.FontResolver = CustomFontResolver.Instance;

builder.Services.AddDbContext<MySqlDbContext>(options =>
    options
        .UseMySql(builder.Configuration.GetConnectionString("MySQLConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySQLConnection")))
        .ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>()
);

var plaidClientId = builder.Configuration["Plaid:ClientId"];
var plaidSecret = builder.Configuration["Plaid:Secret"];
if (string.IsNullOrWhiteSpace(plaidClientId) || string.IsNullOrWhiteSpace(plaidSecret))
{
    throw new InvalidOperationException("Missing Plaid credentials from environment.");
}
var plaidEnvironment = builder.Configuration["Plaid:Environment"];
if (string.IsNullOrWhiteSpace(plaidEnvironment))
{
    throw new InvalidOperationException("Missing Plaid environment from environment.");
}

var stripeSecretKey = builder.Configuration["Stripe:SecretKey"];
var stripePublishableKey = builder.Configuration["Stripe:PublishableKey"];
if (string.IsNullOrWhiteSpace(stripeSecretKey) || string.IsNullOrWhiteSpace(stripePublishableKey))
{
    throw new InvalidOperationException("Missing Stripe credentials from environment.");
}

var paypalClientId = builder.Configuration["PayPal:ClientId"];
var paypalSecret = builder.Configuration["PayPal:Secret"];
if (string.IsNullOrWhiteSpace(paypalClientId) || string.IsNullOrWhiteSpace(paypalSecret))
{
    throw new InvalidOperationException("Missing PayPal credentials from environment.");
}

var encryptionKey = builder.Configuration["EncryptionSettings:Key"];
var encryptionIV = builder.Configuration["EncryptionSettings:IV"];
if (string.IsNullOrWhiteSpace(encryptionKey) || string.IsNullOrWhiteSpace(encryptionIV))
{
    throw new InvalidOperationException("Missing JWT Encryption credentials from environment.");
}

var encryptionDocKey = builder.Configuration["EncryptionDocSettings:Key"];
var encryptionDocIV = builder.Configuration["EncryptionDocSettings:IV"];
if (string.IsNullOrWhiteSpace(encryptionDocKey) || string.IsNullOrWhiteSpace(encryptionDocIV))
{
    throw new InvalidOperationException("Missing Document Encryption credentials from environment.");
}

var qbClientId = builder.Configuration["QB:ClientId"];
var qbClientSecret = builder.Configuration["QB:Secret"];
var qbRedirectUri = builder.Configuration["QB:RedirectUri"];

if (string.IsNullOrWhiteSpace(qbClientId) || string.IsNullOrWhiteSpace(qbClientSecret))
{
    throw new InvalidOperationException("Missing QuickBooks credentials from environment.");
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services
    .AddOptions<PlaidOptions>()
    .Bind(builder.Configuration.GetSection("Plaid"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<StripeOptions>()
    .Bind(builder.Configuration.GetSection("Stripe"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<PayPalOptions>()
    .Bind(builder.Configuration.GetSection("PayPal"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<QuickBooksOptions>()
    .Bind(builder.Configuration.GetSection("QB"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDefaultCorrelationId();

// Optional: also inject PlaidOptions directly
builder.Services.AddSingleton(res =>
    res.GetRequiredService<IOptions<PlaidOptions>>().Value);

// 🧠 If needed elsewhere, register PlaidClient
builder.Services.AddPlaid(builder.Configuration.GetSection("Plaid"));
builder.Services.AddScoped<IPlaidSandboxTestService, PlaidSandboxTestService>();

// ✅ Register Repositories & Services 
builder.Services.Configure<EncryptionJwtSettings>(builder.Configuration.GetSection("EncryptionSettings"));
builder.Services.AddSingleton<EncryptionJwtHelper>();
builder.Services.Configure<EncryptionDocSettings>(builder.Configuration.GetSection("EncryptionDocSettings"));
builder.Services.AddSingleton<EncryptionDocHelper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDocumentReferenceRepository, DocumentReferenceRepository>();
builder.Services.AddScoped<IDocumentReferenceService, DocumentReferenceService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ILeaseRepository, LeaseRepository>();
builder.Services.AddScoped<ILeaseService, LeaseService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IOwnersService, OwnersService>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IPropertyPhotosRepository, PropertyPhotosRepository>();
builder.Services.AddScoped<IPropertyPhotosService, PropertyPhotosService>();
builder.Services.AddScoped<IPricingRepository, PricingRepository>();
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMaintenanceRequestRepository, MaintenanceRequestRepository>();
builder.Services.AddScoped<IMaintenanceRequestService, MaintenanceRequestService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IInvoiceExportService, InvoiceExportService>();
builder.Services.AddScoped<IExportService<InvoiceDto>, InvoiceExportService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<ICardTokenRepository, CardTokenRepository>();
builder.Services.AddScoped<ICardTokenService, CardTokenService>();
builder.Services.AddScoped<IPreferredMethodRepository, PreferredMethodRepository>();
builder.Services.AddScoped<IPreferredMethodService, PreferredMethodService>();
builder.Services.AddScoped<ITenantAnnouncementRepository, TenantAnnouncementRepository>();
builder.Services.AddScoped<ITenantAnnouncementService, TenantAnnouncementService>();
builder.Services.AddScoped<IOwnerAnnouncementRepository, OwnerAnnouncementRepository>();
builder.Services.AddScoped<IOwnerAnnouncementService, OwnerAnnouncementService>();

//PayPal
builder.Services.AddScoped<IPayPalPaymentProcessor, PayPalPaymentProcessor>();
builder.Services.AddScoped<IPayPalRepository, PayPalRepository>();
builder.Services.AddScoped<IPayPalService, PayPalService>();

//Plaid
builder.Services.AddScoped<IPlaidService, PlaidService>();
builder.Services.AddScoped<IPlaidLinkService, PlaidLinkService>();
builder.Services.AddScoped<PaymentAuditLogger>();

//Quickbooks
builder.Services.AddHttpClient<QuickBooksTokenClient>();
builder.Services.AddScoped<QuickBooksPaymentService>();
builder.Services.AddScoped<IStateManager, StateManager>();
builder.Services.AddScoped<IItemReferenceResolver, DefaultItemReferenceResolver>();
builder.Services.AddScoped<IQuickBooksTokenManager, QuickBooksTokenManager>();
builder.Services.AddScoped<IQuickBooksInvoiceService, QuickBooksInvoiceService>();
builder.Services.AddSingleton<ITokenStore, InMemoryTokenStore>();
builder.Services.AddScoped<IQuickBooksUrlService, QuickBooksUrlService>();
builder.Services.AddSingleton<EncryptionDocHelper>();
builder.Services.AddSingleton<EncryptionJwtHelper>();


//Stripe
builder.Services.AddScoped<AuditEventBuilder>();
builder.Services.AddScoped<PaymentAuditLogger>();
builder.Services.AddScoped<IStripeWebhookService, StripeWebhookService>();
builder.Services.AddScoped<PlaidPaymentAuditLogger>();
builder.Services.AddScoped<IStripeRepository, StripeRepository>();
builder.Services.AddScoped<IStripeService, StripeService>(provider =>
{
    var opts = provider.GetRequiredService<IOptions<StripeOptions>>().Value;

    var invoiceRepository = provider.GetRequiredService<IInvoiceRepository>();
    var stripeRepository = provider.GetRequiredService<IStripeRepository>();
    var logger = provider.GetRequiredService<ILogger<StripeService>>();
    var auditLogger = provider.GetRequiredService<PaymentAuditLogger>();

    return new StripeService(
        opts.SecretKey,
        opts.PublishableKey,
        invoiceRepository,
        logger,
        stripeRepository, 
        auditLogger
    );
});

builder.Services.AddSingleton<IOptions<PlaidOptions>>(
    Options.Create(new PlaidOptions
    {
        ClientId = plaidClientId,
        Secret = plaidSecret,
        Environment = plaidEnvironment
    }));

builder.Services.AddScoped<PlaidClient>(provider =>
{
    var plaidOptions = provider.GetRequiredService<IOptions<Going.Plaid.PlaidOptions>>();
    var httpFactory = provider.GetRequiredService<IHttpClientFactory>();
    var logger = provider.GetRequiredService<ILogger<PlaidClient>>();

    return new PlaidClient(plaidOptions, httpFactory, logger);
});

builder.Services.AddScoped<IPlaidService, PlaidService>(provider =>
{       
    var plaidClient = provider.GetRequiredService<PlaidClient>();
    var logger = provider.GetRequiredService<ILogger<PlaidService>>();
    var plaidAuditLogger = provider.GetRequiredService<PlaidPaymentAuditLogger>(); // Updated namespace
    var correlation = provider.GetRequiredService<CorrelationContextAccessor>();
    return new PlaidService(plaidClient, logger, plaidAuditLogger, correlation);
});

builder.Services.AddScoped<IPayPalService, PayPalService>(provider =>
{
    var invoiceRepository = provider.GetRequiredService<IInvoiceRepository>();
    var logger = provider.GetRequiredService<ILogger<PayPalService>>();
    var paymentRepository = provider.GetRequiredService<IPaymentRepository>();
    var payPalPaymentProcessor = provider.GetRequiredService<IPayPalPaymentProcessor>();
    var auditLogger = provider.GetRequiredService<PaymentAuditLogger>();
    var payPalHttpClient = provider.GetRequiredService<PayPalHttpClient>();

    return new PayPalService(
        invoiceRepository,
        logger,
        paymentRepository,
        payPalPaymentProcessor,
        auditLogger,
        payPalHttpClient
    );
});

builder.Services.AddSingleton<QuickBooksTokenClient>();
builder.Services.AddSingleton<QuickBooksTokenManager>();
builder.Services.AddTransient<QuickBooksInvoiceService>();


builder.Services.AddSingleton<PayPalClient>(provider =>
{
    var opts = provider.GetRequiredService<IOptions<PayPalOptions>>().Value;
    return new PayPalClient(opts.ClientId, opts.Secret);
});

builder.Services.AddScoped<PayPalHttpClient>(provider =>
{
    var opts = provider.GetRequiredService<IOptions<PayPalOptions>>().Value;
    var environment = new SandboxEnvironment(opts.ClientId, opts.Secret);
    return new PayPalHttpClient(environment);
});

builder.Services.AddSingleton(new QuickBooksAuthSettings
{
    ClientId = qbClientId,
    ClientSecret = qbClientSecret,
    RedirectUri = qbRedirectUri
});

builder.Services.AddSingleton(res =>
    res.GetRequiredService<IOptions<PlaidOptions>>().Value);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables();

// ✅ Ensure JwtSettings Exists & Generate Test Key If Missing
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
if (!jwtSettingsSection.Exists())
{
    throw new InvalidOperationException("JwtSettings configuration section is missing.");
}

var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
if (string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    Console.WriteLine("⚠️ Warning: SecretKey is missing. Generating a temporary key for testing.");
    jwtSettings.SecretKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}

builder.Services.Configure<JwtSettings>(jwtSettingsSection);
builder.Services.Configure<EncryptionJwtSettings>(builder.Configuration.GetSection("EncryptionSettings"));
builder.Services.Configure<EncryptionDocSettings>(builder.Configuration.GetSection("EncryptionDocSettings"));

// ✅ Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

// ✅ Enable Authorization Middleware (Role-Based Access Control)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
    options.AddPolicy("ViewerPolicy", policy => policy.RequireRole("Viewer"));
});

// ✅ Log Configuration Sections
foreach (var section in builder.Configuration.GetChildren())
{
    Console.WriteLine($"🔹 Found Configuration Section: {section.Key}");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Adjust to match your Angular port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ Add Controllers & API Documentation (Swagger)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // 👈 Add this line
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Property Management API", Version = "v1" });

    c.DocInclusionPredicate((_, apiDesc) =>
    {
        var controllerNamespace = apiDesc.ActionDescriptor?.RouteValues["controller"];
        return !apiDesc.ActionDescriptor?.DisplayName?.Contains(".Test.") ?? true;
    });

    // Optional: Use full type names for schema IDs to avoid duplicates
    c.CustomSchemaIds(type => type.FullName);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.Configure<PlaidSettings>(
    builder.Configuration.GetSection("Plaid"));

builder.Services.Configure<QuickBooksAuthSettings>(
    builder.Configuration.GetSection("QB"));

builder.Services.Configure<PayPalSettings>(
    builder.Configuration.GetSection("PayPal"));

builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe"));

builder.Services.Configure<EncryptionDocSettings>(
    builder.Configuration.GetSection("EncryptionDocSettings"));

var app = builder.Build();

app.UseCors("AllowFrontend"); // ✅ Apply the specific CORS policy

app.UseRouting();
app.UseCorrelationId();
app.UseHttpsRedirection();
app.UseCors("AllowAll");

// ✅ Enable Authentication Middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RoleMiddleware>();

// ✅ Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Main API");
    });
}

app.MapControllers();
app.Run();
public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime) =>
        new { Type = context.GetType(), designTime };
}