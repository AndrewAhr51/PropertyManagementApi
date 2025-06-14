using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Domain.Entities;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Create a new user
        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // ✅ Retrieve a user by ID
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // ✅ Retrieve a user by username
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        // ✅ Retrieve a user by email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // ✅ Update user details
        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        // ✅ Remove a user by ID
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        // ✅ Retrieve role ID by role name
        public async Task<int> GetRoleIdAsync(string role)
        {
            return await _context.Roles
                .Where(r => r.Name == role)
                .Select(r => r.RoleId)
                .FirstOrDefaultAsync();
        }

        // 🔹 Forgot Password Methods
        public async Task<bool> StoreResetTokenAsync(string email, string token, DateTimeOffset expiration)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return false;

            user.ResetToken = token;
            user.ResetTokenExpiration = expiration;

            return await UpdateAsync(user);
        }

        public async Task<string?> GetResetTokenAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email && u.ResetTokenExpiration > DateTimeOffset.UtcNow)
                .Select(u => u.ResetToken)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteResetTokenAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return false;

            user.ResetToken = null;
            user.ResetTokenExpiration = null;

            return await UpdateAsync(user);
        }

        // 🔹 Multi-Factor Authentication (MFA) Methods
        public async Task<bool> StoreMfaCodeAsync(string email, string code, DateTimeOffset expiration)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return false;

            user.MfaCode = code;
            user.MfaCodeExpiration = expiration;

            return await UpdateAsync(user);
        }

        public async Task<string?> GetMfaCodeAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email && u.MfaCodeExpiration > DateTimeOffset.UtcNow)
                .Select(u => u.MfaCode)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteMfaCodeAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return false;

            user.MfaCode = null;
            user.MfaCodeExpiration = null;

            return await UpdateAsync(user);
        }

        // 🔹 Password Update Method
        public async Task<bool> UpdateUserPasswordAsync(string email, string newPassword)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword); // ✅ Secure hashing
            return await UpdateAsync(user);
        }


        public async Task<DateTimeOffset?> GetMfaCodeExpirationAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.MfaCodeExpiration)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> EnableMfaAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return false;

            user.IsMfaEnabled = true;
            return await UpdateAsync(user);
        }

        public async Task<bool> DisableMfaAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return false;

            user.IsMfaEnabled = false;
            return await UpdateAsync(user);
        }

        public async Task<bool> StoreResetTokenAsync(string email, string token, DateTime expiration)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return false;

            user.ResetToken = token;
            user.ResetTokenExpiration = expiration;

            return await UpdateAsync(user);
        }

        public async Task<DateTime?> GetTokenExpirationAsync(string email)
        {
            var expiration = await _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.ResetTokenExpiration)
                .FirstOrDefaultAsync();

            return expiration.HasValue ? expiration.Value.DateTime : (DateTime?)null;
        }

    }
}