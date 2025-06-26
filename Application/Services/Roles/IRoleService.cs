using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Application.Services.Roles
{
    public interface IRoleService
    {
        Task<RolesDto> CreateRoleAsync(CreateRoleDto roleDto);
        Task<RolesDto?> GetRoleByIdAsync(int id);
        Task<bool> UpdateRoleAsync(int id, CreateRoleDto roleDto);
        Task<bool> DeleteRoleAsync(int id);
    }
}
