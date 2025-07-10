using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using PropertyManagementAPI.Application.Services.Email;
using PropertyManagementAPI.Domain.DTOs.Users;

namespace PropertyManagementAPI.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;


        public AuthService(
            IConfiguration configuration,
            IUserRepository userRepository,
            IEmailService emailService,
            ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }


        // ✅ Authenticate user and generate JWT token
        public async Task<string?> AuthenticateAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(loginDto.UserName);
                if (user is null || !VerifyPassword(user.PasswordHash, loginDto.Password))
                {
                    _logger.LogWarning("Authentication failed for user {UserName}.", loginDto.UserName);
                    return null;
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("Authentication succeeded for user {UserName}.", loginDto.UserName);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for user {UserName}.", loginDto.UserName);
                throw;
            }
        }

        private string GenerateJwtToken(Domain.Entities.User.Users user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("SecretKey is not configured.");
            var issuer = _configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("Issuer is not configured.");
            var audience = _configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("Audience is not configured.");
            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? throw new InvalidOperationException("ExpirationMinutes is not configured."));

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

        // ✅ Refresh JWT token
        public async Task<string?> RefreshTokenAsync(string expiredToken)
        {
            var principal = GetPrincipalFromExpiredToken(expiredToken);
            if (principal == null) return null;

            var username = principal.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return null;

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user is null) return null;

            return GenerateJwtToken(user);
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

        // ✅ Password Reset Implementation
        public async Task<bool> SendResetEmailAsync(EmailDto emailDto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(emailDto.EmailAddress);
                if (user is null)
                {
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", emailDto.EmailAddress);
                    return false;
                }

                var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                await _userRepository.StoreResetTokenAsync(emailDto.EmailAddress, token, DateTime.UtcNow.AddMinutes(30));

                var resetLink = $"https://yourapp.com/reset-password?token={token}&email={emailDto.EmailAddress}";
                emailDto.Subject = "Password Reset";
                emailDto.Body = $"Click here to reset your password: {resetLink}";

                var success = await _emailService.SendEmailAsync(emailDto);
                if (!success)
                {
                    _logger.LogWarning("Failed to send password reset email to {Email}.", emailDto.EmailAddress);
                    return false;
                }

                _logger.LogInformation("Password reset email sent to {Email}.", emailDto.EmailAddress);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending password reset email to {Email}.", emailDto.EmailAddress);
                return false;
            }
        }

        public async Task<bool> ValidateResetTokenAsync(string email, string token)
        {
            var storedToken = await _userRepository.GetResetTokenAsync(email);
            var expiration = await _userRepository.GetTokenExpirationAsync(email);

            return storedToken == token && expiration > DateTime.UtcNow;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            if (!await ValidateResetTokenAsync(email, token)) return false;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return await _userRepository.UpdateUserPasswordAsync(email, hashedPassword);
        }

        public async Task<bool> InvalidateResetTokenAsync(string email)
        {
            return await _userRepository.DeleteResetTokenAsync(email);
        }

        // ✅ Multi-Factor Authentication (MFA)
        public async Task<string?> GenerateMfaCodeAsync(EmailDto emailDto)
        {
            var user = await _userRepository.GetByEmailAsync(emailDto.EmailAddress);
            if (user is null) return null;

            var code = new Random().Next(100000, 999999).ToString();
            await _userRepository.StoreMfaCodeAsync(emailDto.EmailAddress, code, DateTime.UtcNow.AddMinutes(5));

            emailDto.Subject = "Your MFA Code";
            emailDto.Body = $"Your multi-factor authentication code is: {code}";

            return await _emailService.SendEmailAsync(emailDto) ? code : null;
        }

        public async Task<bool> ValidateMfaCodeAsync(EmailDto emailDto, string code)
        {
            var storedCode = await _userRepository.GetMfaCodeAsync(emailDto.EmailAddress);
            var expiration = await _userRepository.GetMfaCodeExpirationAsync(emailDto.EmailAddress);

            return storedCode == code && expiration > DateTime.UtcNow;
        }

        public async Task<bool> EnableMfaAsync(EmailDto emailDto)
        {
            return await _userRepository.EnableMfaAsync(emailDto.EmailAddress);
        }

        public async Task<bool> DisableMfaAsync(EmailDto emailDto)
        {
            return await _userRepository.DisableMfaAsync(emailDto.EmailAddress);
        }

        public async Task<string?> GenerateResetTokenAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null) return null;

            // ✅ Generate a secure reset token
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            // ✅ Store the token with an expiration time (e.g., 30 minutes)
            await _userRepository.StoreResetTokenAsync(email, token, DateTime.UtcNow.AddMinutes(30));

            return token;
        }
    }
}