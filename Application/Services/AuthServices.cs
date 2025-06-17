using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Application.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public AuthService(IConfiguration configuration, IUserRepository userRepository, IEmailService emailService)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<string?> AuthenticateAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.UserName);
        if (user is null || !VerifyPassword(user.PasswordHash, loginDto.Password))
            return null;

        // Convert User? to Users (if needed)
        if (user is not User usersEntity)
            throw new InvalidCastException("User repository did not return a Users entity.");

        return GenerateJwtToken(usersEntity);
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("SecretKey is not configured.");
        var issuer = _configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("Issuer is not configured.");
        var audience = _configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("Audience is not configured.");
        var expirationMinutesStr = _configuration["JwtSettings:ExpirationMinutes"] ?? throw new InvalidOperationException("ExpirationMinutes is not configured.");
        var expirationMinutes = int.Parse(expirationMinutesStr);

        var key = Encoding.UTF8.GetBytes(secretKey);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string storedHash, string providedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, storedHash);
    }

    public async Task<string?> RefreshTokenAsync(string expiredToken)
    {
        var principal = GetPrincipalFromExpiredToken(expiredToken);
        if (principal == null) return null;

        var username = principal.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return null;

        var user = await _userRepository.GetByUsernameAsync(username);
        if (user is null) return null;

        if (user is not User usersEntity)
            throw new InvalidCastException("User repository did not return a Users entity.");

        return GenerateJwtToken(usersEntity);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("SecretKey is not configured.");
        var issuer = _configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("Issuer is not configured.");
        var audience = _configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("Audience is not configured.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }

    // 🔹 Forgot Password Implementation
    public async Task<string?> GenerateResetTokenAsync(EmailDto email)
    {
        var user = await _userRepository.GetByEmailAsync(email.EmailAddress);
        if (user is null) return null;

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        await _userRepository.StoreResetTokenAsync(email.EmailAddress, token, DateTime.UtcNow.AddMinutes(30));
        return token;
    }

    public async Task<bool> SendResetEmailAsync(EmailDto email)
    {
        var token = await GenerateResetTokenAsync(email); // Fix: Pass the EmailDto object instead of email.EmailAddress
        if (token == null) return false;

        var resetLink = $"https://yourapp.com/reset-password?token={token}&email={email.EmailAddress}";
        return await _emailService.SendEmailAsync(email.EmailAddress, "Password Reset", $"Click here to reset your password: {resetLink}", 1);
    }

    public async Task<bool> ValidateResetTokenAsync(EmailDto email, string token)
    {
        var storedToken = await _userRepository.GetResetTokenAsync(email.EmailAddress);
        var expiration = await _userRepository.GetTokenExpirationAsync(email.EmailAddress);

        return storedToken == token && expiration > DateTime.UtcNow;
    }

    public async Task<bool> ResetPasswordAsync(EmailDto email, string token, string newPassword)
    {
        if (!await ValidateResetTokenAsync(email, token)) return false;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
        return await _userRepository.UpdateUserPasswordAsync(email.EmailAddress, hashedPassword);
    }

    public async Task<bool> InvalidateResetTokenAsync(EmailDto email)
    {
        return await _userRepository.DeleteResetTokenAsync(email.EmailAddress);
    }

    // 🔹 Multi-Factor Authentication (MFA)
    public async Task<string?> GenerateMfaCodeAsync(EmailDto email)
    {
        var user = await _userRepository.GetByEmailAsync(email.EmailAddress);
        if (user is null) return null;

        var code = new Random().Next(100000, 999999).ToString();
        await _userRepository.StoreMfaCodeAsync(email.EmailAddress, code, DateTime.UtcNow.AddMinutes(5));
        return code;
    }

    public async Task<bool> ValidateMfaCodeAsync(EmailDto email, string code)
    {
        var storedCode = await _userRepository.GetMfaCodeAsync(email.EmailAddress);
        var expiration = await _userRepository.GetMfaCodeExpirationAsync(email.EmailAddress);

        return storedCode == code && expiration > DateTime.UtcNow;
    }

    public async Task<bool> EnableMfaAsync(EmailDto email)
    {
        return await _userRepository.EnableMfaAsync(email.EmailAddress);
    }

    public async Task<bool> DisableMfaAsync(EmailDto email)
    {
        return await _userRepository.DisableMfaAsync(email.EmailAddress);
    }
}