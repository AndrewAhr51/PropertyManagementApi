using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto userDto);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id, UpdateUserDto userDto);
        Task<UserDto?> GetByEmailAsync(string email); // ✅ New method for retrieving user by email
        Task<bool> DeleteUserAsync(int id);
        
    }
}