using PropertyManagementAPI.Domain.DTOs.Roles;

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
