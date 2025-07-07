using BCrypt.Net;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Intuit.Ipp.Data;
using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Repositories.Users
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
        public async Task<Domain.Entities.User.Users> AddAsync(Domain.Entities.User.Users user)
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

        public async Task<List<Domain.Entities.User.Users>> GetAllUsersAsync()
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
        public async Task<Domain.Entities.User.Users?> GetByIdAsync(int id)
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
        public async Task<Domain.Entities.User.Users?> GetByUsernameAsync(string username)
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
        public async Task<Domain.Entities.User.Users?> GetByEmailAsync(string email)
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
        public async Task<bool> UpdateAsync(Domain.Entities.User.Users user)
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
        public async Task<bool> SetActivateUser(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {Id}", userId);
                    return false;
                }

                user.IsActive = !user.IsActive;

                var owner = await _context.Owners.FindAsync(userId);
                if (owner != null)
                {
                    owner.IsActive = !owner.IsActive;
                }

                var tenant = await _context.Tenants.FindAsync(userId);
                if (tenant != null)
                {
                    tenant.IsActive = !tenant.IsActive;
                }

                var saveCount = await _context.SaveChangesAsync();
                return saveCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling activation for user ID: {Id}", userId);
                throw;
            }
        }

        public async Task<bool> SetActivateUserByEmailAsync(string email)
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

                // ✅ Toggle IsActive to the opposite value
                user.IsActive = !user.IsActive;

                var isActive = user.IsActive;

                var owner = await _context.Owners.FindAsync(user.UserId);
                if (owner != null)
                {
                    owner.IsActive = !owner.IsActive;
                }

                var tenant = await _context.Tenants.FindAsync(user.UserId);
                if (tenant != null)
                {
                    tenant.IsActive = !tenant.IsActive;
                }

                var save = await _context.SaveChangesAsync();

                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating the user with ID: {email}", email);
                throw new Exception("An error occurred while deactivating the user.", ex);
            }
        }
        public async Task<bool> SetActivateUserByUsernameAsync(string username)
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

                // ✅ Toggle IsActive to the opposite value
                user.IsActive = !user.IsActive;

                var isActive = user.IsActive;

                var owner = await _context.Owners.FindAsync(user.UserId);
                if (owner != null)
                {
                    owner.IsActive = !owner.IsActive;
                }

                var tenant = await _context.Tenants.FindAsync(user.UserId);
                if (tenant != null)
                {
                    tenant.IsActive = !tenant.IsActive;
                }

                var save = await _context.SaveChangesAsync();

                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating the user with {username}", username);
                throw new Exception("An error occurred while deactivating the user.", ex);
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

                return expiration.HasValue ? expiration.Value.DateTime : null;
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

                var save = await _context.SaveChangesAsync();

                // ✅ Step 2: Check user role and update accordingly
                _logger.LogInformation("User with ID {UserId} is now {IsActive}.", userId, isActive ? "active" : "inactive");
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == user.RoleId);
                if (role == null)
                {
                    _logger.LogWarning("Role not found for User ID: {UserId}", userId);
                    return false; // Role not found
                }

                if (role.Name == "Owner") 
                {
                    _logger.LogInformation("User with ID {UserId} is an owner. Updating owner id and related properties.", userId);
                    return await SetActivateOwnerAsync(userId, isActive);
                }
                else if (role.Name == "Tenant") 
                    {
                    _logger.LogInformation("User with ID {UserId} is a tenant. Update the tenant id; No property updates required.", userId);
                    return await SetActivateTenantAsync(userId, isActive);
                }

                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating/deactivating user with ID: {UserId}", userId);
                throw new Exception("An error occurred while activating/deactivating the user.", ex);
            }
        }

        public async Task<bool> SetActivateOwnerAsync(int userId, bool isActive)
        {
            var owner = await _context.Owners.FirstOrDefaultAsync(o => o.OwnerId == userId);
            if (owner == null) return false;

            owner.IsActive = isActive;
            await _context.SaveChangesAsync();

            // ✅ Step 3: Get all PropertyIds owned by this owner
            var propertyIds = await _context.PropertyOwners
                .Where(po => po.OwnerId == owner.OwnerId)
                .Select(po => po.PropertyId)
                .ToListAsync();

            if (propertyIds == null || propertyIds.Count == 0)
            {
                _logger.LogWarning("No properties found for owner with ID: {OwnerId}", owner.OwnerId);
                return false;
            }
            else
            {
                _logger.LogInformation("Found {Count} properties for owner with ID: {OwnerId}", propertyIds.Count, owner.OwnerId);
            }
            foreach (var propertyId in propertyIds)
            {
                _logger.LogInformation("Property ID: {PropertyId}", propertyId);
                var property = await _context.Properties.FindAsync(propertyId);
                if (property == null)
                {
                    _logger.LogWarning("Property with ID {PropertyId} not found.", propertyId);
                    continue; // Skip to the next property if not found
                }
                _logger.LogInformation("Updating property with ID: {PropertyId} to IsActive: {IsActive}", propertyId, isActive);
                property.IsActive = isActive; // Update IsActive status
            }

            // ✅ Step 4: Save changes to properties
            var save = await _context.SaveChangesAsync();

            return save > 0;
        }

        public async Task<bool> SetActivateTenantAsync(int userId, bool isActive)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(o => o.TenantId == userId);
            if (tenant == null) return false;

            tenant.IsActive = isActive;
            var save = await _context.SaveChangesAsync();

            return save > 0;
        }
    }
}