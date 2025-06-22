using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PropertyManagementAPI.API.Mapping;
using PropertyManagementAPI.Application.Configuration;
using PropertyManagementAPI.Application.Repositories.Invoices;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Common.Utilities;
using PropertyManagementAPI.Domain.Entities;
//
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//// ✅ Configure SQL Server Database Context
//builder.Services.AddDbContext<SQLServerDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection")));

builder.Services.AddDbContext<MySqlDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MySQLConnection"),
    new MySqlServerVersion(new Version(8, 0, 32))));

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
builder.Services.AddScoped<IRentInvoiceRepository, RentInvoiceRepository>();
builder.Services.AddScoped<IRentalInvoiceService, RentalInvoiceService>();
builder.Services.AddScoped<IUtilityInvoiceRepository, UtilityInvoiceRepository>();
builder.Services.AddScoped<IUtilityInvoiceService, UtilityInvoiceService>();
builder.Services.AddScoped<ICreditCardInfoRepository, CreditCardInfoRepository>();
builder.Services.AddScoped<ICreditCardInfoService, CreditCardInfoService>();
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
builder.Services.AddScoped<IPaymentMethodsRepository, PaymentMethodsRepository>();
builder.Services.AddScoped<IPaymentMethodsService, PaymentMethodsService>();
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

builder.Services.AddControllers();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

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