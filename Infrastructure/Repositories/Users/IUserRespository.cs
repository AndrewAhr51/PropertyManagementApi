using PropertyManagementAPI.Domain.Entities.User;

public interface IUserRepository
{
    Task<Users> AddAsync(Users user);  // ✅ Create a new user
    Task<List<Users>> GetAllUsersAsync();  // ✅ Retrieve all users
    Task<Users?> GetByIdAsync(int id);  // ✅ Retrieve a user by ID
    Task<Users?> GetByUsernameAsync(string username);  // ✅ Retrieve a user by username
    Task<Users?> GetByEmailAsync(string email);  // ✅ Retrieve a user by email
    Task<bool> UpdateAsync(Users user);  // ✅ Update user details
    Task<bool> SetActivateUser(int id);  // ✅ Remove a user by ID
    Task<bool> SetActivateUserByEmailAsync(string email);
    Task<bool> SetActivateUserByUsernameAsync(string username);
    Task<int> GetRoleIdAsync(string role);  // ✅ Retrieve role ID by role name    
    Task<bool> SetActivateUserAsync(int id); 

    // 🔹 Forgot Password Methods
    Task<bool> StoreResetTokenAsync(string email, string token, DateTime expiration); // ✅ Stores reset token securely
    Task<string?> GetResetTokenAsync(string email);  // ✅ Retrieves stored reset token
    Task<DateTime?> GetTokenExpirationAsync(string email);  // ✅ Retrieves token expiration time
    Task<bool> DeleteResetTokenAsync(string email);  // ✅ Deletes reset token after reset

    // 🔹 Multi-Factor Authentication (MFA) Methods
    Task<bool> StoreMfaCodeAsync(string email, string code, DateTimeOffset expiration);
    Task<string?> GetMfaCodeAsync(string email);
    Task<DateTimeOffset?> GetMfaCodeExpirationAsync(string email);
    Task<bool> EnableMfaAsync(string email);
    Task<bool> DisableMfaAsync(string email);

    // 🔹 Password Update Method
    Task<bool> UpdateUserPasswordAsync(string email, string newPasswordHash);



}