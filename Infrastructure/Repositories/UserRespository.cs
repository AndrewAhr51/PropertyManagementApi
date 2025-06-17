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
        private readonly MySqlDbContext _context;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(MySqlDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        // ✅ Create a new user
        public async Task<User> AddAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {UserName}", user.UserName);
                throw new Exception("An error occurred while adding the user.", ex);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users.");
                throw new Exception("An error occurred while retrieving users.", ex);
            }
        }

        // ✅ Retrieve a user by ID
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID: {Id}", id);
                throw new Exception("An error occurred while retrieving the user.", ex);
            }
        }

        // ✅ Retrieve a user by username
        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    _logger.LogWarning("Username is null or empty.");
                    return null;
                }
                else
                {
                    return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by username: {Username}", username);
                throw new Exception("An error occurred while retrieving the user.", ex);
            }

        }

        // ✅ Retrieve a user by email
        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return null;
                }
                else
                {
                    return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email: {Email}", email);
                throw new Exception("An error occurred while retrieving the user.", ex);
            }

        }

        // ✅ Update user details
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    _logger.LogWarning("Attempted to update a null user.");
                    return false;
                }
                // Hash the password if it has been changed
                if (!string.IsNullOrEmpty(user.PasswordHash) && !BCrypt.Net.BCrypt.Verify(user.PasswordHash, user.PasswordHash))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                }

                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserName}", user.UserName);
                throw new Exception("An error occurred while updating the user.", ex);
            }
        }

        // ✅ Remove a user by ID
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid user ID: {Id}", id);
                    return false;
                }
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {Id}", id);
                    return false;
                }
                _context.Users.Remove(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {Id}", id);
                throw new Exception("An error occurred while deleting the user.", ex);
            }
        }

        public async Task<bool> DeleteByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return false;
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null) return false;

                _context.Users.Remove(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user by email: {Email}", email);
                throw new Exception("An error occurred while deleting the user.", ex);
            }
        }

        public async Task<bool> DeleteUserByUsernameAsync(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    _logger.LogWarning("Username is null or empty.");
                    return false;
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
                if (user == null) return false;

                _context.Users.Remove(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user by username: {Username}", username);
                throw new Exception("An error occurred while deleting the user.", ex);
            }
        }
        // ✅ Retrieve role ID by role name
        public async Task<int> GetRoleIdAsync(string role)
        {
            try
            {
                if (string.IsNullOrEmpty(role))
                {
                    _logger.LogWarning("Role is null or empty.");
                    return 0; // Return 0 or an appropriate default value
                }

                return await _context.Roles
               .Where(r => r.Name == role)
               .Select(r => r.RoleId)
               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role ID for role: {Role}", role);
                throw new Exception("An error occurred while retrieving the role ID.", ex);
            }
        }

        // 🔹 Forgot Password Methods
        public async Task<bool> StoreResetTokenAsync(string email, string token, DateTimeOffset expiration)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return false;
                }
                var user = await GetByEmailAsync(email);
                if (user == null) return false;

                user.ResetToken = token;
                user.ResetTokenExpiration = expiration;

                return await UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing reset token for email: {Email}", email);
                throw new Exception("An error occurred while storing the reset token.", ex);
            }
        }

        public async Task<string?> GetResetTokenAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return null;
                }

                return await _context.Users
                .Where(u => u.Email == email && u.ResetTokenExpiration > DateTimeOffset.UtcNow)
                .Select(u => u.ResetToken)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reset token for email: {Email}", email);
                throw new Exception("An error occurred while retrieving the reset token.", ex);
            }
        }

        public async Task<bool> DeleteResetTokenAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return false;
                }
                var user = await GetByEmailAsync(email);
                if (user == null) return false;
                user.ResetToken = null;
                user.ResetTokenExpiration = null;
                return await UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reset token for email: {Email}", email);
                throw new Exception("An error occurred while deleting the reset token.", ex);
            }
        }

        // 🔹 Multi-Factor Authentication (MFA) Methods
        public async Task<bool> StoreMfaCodeAsync(string email, string code, DateTimeOffset expiration)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
                {
                    _logger.LogWarning("Email or code is null or empty.");
                    return false;
                }

                var user = await GetByEmailAsync(email);
                if (user == null) return false;

                user.MfaCode = code;
                user.MfaCodeExpiration = expiration;

                return await UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing MFA code for email: {Email}", email);
                throw new Exception("An error occurred while storing the MFA code.", ex);
            }
        }

        public async Task<string?> GetMfaCodeAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return null;
                }
                return await _context.Users
                .Where(u => u.Email == email && u.MfaCodeExpiration > DateTimeOffset.UtcNow)
                .Select(u => u.MfaCode)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving MFA code for email: {Email}", email);
                throw new Exception("An error occurred while retrieving the MFA code.", ex);
            }
        }

        public async Task<bool> DeleteMfaCodeAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return false;
                }
                var user = await GetByEmailAsync(email);

                if (user == null) return false;

                user.MfaCode = null;
                user.MfaCodeExpiration = null;

                return await UpdateAsync(user);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting MFA code for email: {Email}", email);
                throw new Exception("An error occurred while deleting the MFA code.", ex);
            }
        }

        // 🔹 Password Update Method
        public async Task<bool> UpdateUserPasswordAsync(string email, string newPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
                {
                    _logger.LogWarning("Email or new password is null or empty.");
                    return false;
                }

                var user = await GetByEmailAsync(email);
                if (user == null) return false;

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword); // ✅ Secure hashing
                return await UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user password for email: {Email}", email);
                throw new Exception("An error occurred while updating the user password.", ex);
            }
        }

        public async Task<DateTimeOffset?> GetMfaCodeExpirationAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return null;
                }
                return await _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.MfaCodeExpiration)
                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving MFA code expiration for email: {Email}", email);
                throw new Exception("An error occurred while retrieving the MFA code expiration.", ex);
            }
        }

        public async Task<bool> EnableMfaAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return false;
                }
                var user = await GetByEmailAsync(email);
                if (user == null) return false;

                user.IsMfaEnabled = true;
                return await UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling MFA for email: {Email}", email);
                throw new Exception("An error occurred while enabling MFA.", ex);
            }
        }

        public async Task<bool> DisableMfaAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return false;
                }
                var user = await GetByEmailAsync(email);
                if (user == null) return false;

                user.IsMfaEnabled = false;
                return await UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling MFA for email: {Email}", email);
                throw new Exception("An error occurred while disabling MFA.", ex);
            }

        }

        public async Task<bool> StoreResetTokenAsync(string email, string token, DateTime expiration)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Email or token is null or empty.");
                    return false;
                }

                var user = await GetByEmailAsync(email);
                if (user == null) return false;

                user.ResetToken = token;
                user.ResetTokenExpiration = expiration;

                return await UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing reset token for email: {Email}", email);
                throw new Exception("An error occurred while storing the reset token.", ex);
            }
        }

        public async Task<DateTime?> GetTokenExpirationAsync(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Email is null or empty.");
                    return null;
                }
                var expiration = await _context.Users
                .Where(u => u.Email == email)
                .Select(u => u.ResetTokenExpiration)
                .FirstOrDefaultAsync();

                return expiration.HasValue ? expiration.Value.DateTime : (DateTime?)null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving token expiration for email: {Email}", email);
                throw new Exception("An error occurred while retrieving the token expiration.", ex);
            }
        }

        public async Task<bool> SetActivateUserAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    _logger.LogWarning("Invalid user ID: {UserId}", userId);
                    return false;
                }
                // ✅ Step 1: Toggle user
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.IsActive = !user.IsActive;

                var isActive = user.IsActive; // Store the new state

                await _context.SaveChangesAsync();

                // ✅ Step 2: Find owner by UserId
                var owner = await _context.Owners.FirstOrDefaultAsync(o => o.UserId == userId);
                if (owner == null) return false;

                owner.IsActive = isActive;
                await _context.SaveChangesAsync();

                // ✅ Step 3: Get all PropertyIds owned by this owner
                var propertyIds = await _context.PropertyOwners
                    .Where(po => po.OwnerId == owner.OwnerId)
                    .Select(po => po.PropertyId)
                    .ToListAsync();

                // ✅ Step 4: Update IsActive on each property
                var properties = await _context.Property
                    .Where(p => propertyIds.Contains(p.PropertyId))
                    .ToListAsync();

                foreach (var property in properties)
                {
                    property.IsActive = isActive;
                }

                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating/deactivating user with ID: {UserId}", userId);
                throw new Exception("An error occurred while activating/deactivating the user.", ex);
            }
        }
    }
}