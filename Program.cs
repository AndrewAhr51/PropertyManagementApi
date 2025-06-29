using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PdfSharp.Fonts;
using PropertyManagementAPI.API.Mapping;
using PropertyManagementAPI.Application.Configuration;
using PropertyManagementAPI.Application.Services.Auth;
using PropertyManagementAPI.Application.Services.Documents;
using PropertyManagementAPI.Application.Services.Email;
using PropertyManagementAPI.Application.Services.InvoiceExport;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Application.Services.Notes;
using PropertyManagementAPI.Application.Services.Owners;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Application.Services.Property;
using PropertyManagementAPI.Application.Services.Roles;
using PropertyManagementAPI.Application.Services.Users;
using PropertyManagementAPI.Application.Services.Vendors;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Common.Utilities;
using PropertyManagementAPI.Domain.DTOs.Invoice;
//
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Documents;
using PropertyManagementAPI.Infrastructure.Repositories.Email;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Notes;
using PropertyManagementAPI.Infrastructure.Repositories.Owners;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using PropertyManagementAPI.Infrastructure.Repositories.Property;
using PropertyManagementAPI.Infrastructure.Repositories.Roles;
using PropertyManagementAPI.Infrastructure.Repositories.Users;
using PropertyManagementAPI.Infrastructure.Repositories.Vendors;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

GlobalFontSettings.FontResolver = CustomFontResolver.Instance;

builder.Services.AddDbContext<MySqlDbContext>(options =>
    options
        .UseMySql(builder.Configuration.GetConnectionString("MySQLConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySQLConnection")))
        .ReplaceService<IModelCacheKeyFactory, DynamicModelCacheKeyFactory>()
);

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// ✅ Register Repositories & Services 
builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection("EncryptionSettings"));
builder.Services.AddSingleton<EncryptionHelper>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDocumentStorageRepository, DocumentStorageRepository>();
builder.Services.AddScoped<IDocumentStorageService, DocumentStorageService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IRentInvoiceRepository, RentInvoiceRepository>();
builder.Services.AddScoped<IRentalInvoiceService, RentalInvoiceService>();
builder.Services.AddScoped<IUtilityInvoiceRepository, UtilityInvoiceRepository>();
builder.Services.AddScoped<IUtilityInvoiceService, UtilityInvoiceService>();
builder.Services.AddScoped<ILeaseRepository, LeaseRepository>();
builder.Services.AddScoped<ILeaseService, LeaseService>();
builder.Services.AddScoped<ILeaseTerminationInvoiceRepository, LeaseTerminationInvoiceRepository>();
builder.Services.AddScoped<ILeaseTerminationInvoiceService, LeaseTerminationInvoiceService>();
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
builder.Services.AddScoped<ISecurityDepositInvoiceRepository, SecurityDepositInvoiceRepository>();
builder.Services.AddScoped<ISecurityDepositInvoiceService, SecurityDepositInvoiceService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<ICleaningFeeInvoiceService, CleaningFeeInvoiceService>(); 
builder.Services.AddScoped<ICleaningFeeInvoiceRepository, CleaningFeeInvoiceRepository>();
builder.Services.AddScoped<IPropertyTaxInvoiceService, PropertyTaxInvoiceService>();
builder.Services.AddScoped<IPropertyTaxInvoiceRepository, PropertyTaxInvoiceRepository>();
builder.Services.AddScoped<ILegalFeeInvoiceRepository, LegalFeeInvoiceRepository>();
builder.Services.AddScoped<ILegalFeeInvoiceService, LegalFeeInvoiceService>();
builder.Services.AddScoped<IInvoiceExportService, InvoiceExportService>();
builder.Services.AddScoped<IExportService<CumulativeInvoiceDto>, InvoiceExportService>();
builder.Services.AddScoped<ICumulativeInvoicesRepository, CumulativeInvoicesRepository>();
builder.Services.AddScoped<ICummulativeInvoicesService, CummulativeInvoicesService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IBankAccountService, BankAccountService>();
builder.Services.AddScoped<ICardTokenRepository, CardTokenRepository>();
builder.Services.AddScoped<ICardTokenService, CardTokenService>();
builder.Services.AddScoped<IPreferredMethodRepository, PreferredMethodRepository>();
builder.Services.AddScoped<IPreferredMethodService, PreferredMethodService>();

builder.Services.AddSingleton<PayPalClient>();
builder.Services.AddScoped<IPaymentProcessor, PayPalPaymentProcessor>();

builder.Services.AddScoped<PaymentAuditLogger>();

builder.Services.AddControllers();

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

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
builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection("EncryptionSettings"));

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

// ✅ Add Controllers & API Documentation (Swagger)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Enable Authentication Middleware
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RoleMiddleware>();

app.MapControllers();
app.Run();

public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime) =>
        new { Type = context.GetType(), designTime };
}