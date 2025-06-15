using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto userDto);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserAsync(int id, UpdateUserDto userDto);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> DeleteUserByEmailAsync(string email);
        Task<bool> DeleteUserByUsernameAsync(string username);
        Task<bool> SetActivateUserAsync(int propertyId);
    }
}