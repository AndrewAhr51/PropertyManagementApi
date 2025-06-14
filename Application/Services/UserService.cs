using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // ✅ Create a new user
        public async Task<UserDto> CreateUserAsync(CreateUserDto userDto)
        {
            var hashedPassword = HashPassword(userDto.Password);
            var roleId = await GetRoleId(userDto.Role);

            var newUser = new Users
            {
                UserName = userDto.Username,
                Email = userDto.Email,
                PasswordHash = hashedPassword,
                RoleId = roleId
            };

            var createdUser = await _userRepository.AddAsync(newUser);
            return new UserDto { UserId = createdUser.UserId, UserName = createdUser.UserName, Email = createdUser.Email, Role = createdUser.RoleId };
        }

        // ✅ Retrieve a user by ID
        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;
            return new UserDto { UserId = user.UserId, UserName = user.UserName, Email = user.Email, Role = user.RoleId };
        }

        // ✅ Update user details
        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.UserName = userDto.Username;
            user.Email = userDto.Email;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.PasswordHash = HashPassword(userDto.Password);
            }

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.RoleId,
                IsActive = user.IsActive,
                DateCreated = DateTime.UtcNow
            };
        }

        // ✅ Delete a user by ID
        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        private async Task<int> GetRoleId(string role)
        {
            return await _userRepository.GetRoleIdAsync(role);
        }
    }
}