using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PropertyManagementAPI.API.Mapping;
using PropertyManagementAPI.Application.Configuration;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure SQL Server Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NCFConnection")));

// ✅ Register Repositories & Services 
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IOwnersService, OwnersService>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

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