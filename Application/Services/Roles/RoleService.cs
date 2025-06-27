using PropertyManagementAPI.Domain.DTOs.Roles;
using PropertyManagementAPI.Domain.Entities.Roles;
using PropertyManagementAPI.Infrastructure.Repositories.Roles;

namespace PropertyManagementAPI.Application.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // ✅ Create a role
        public async Task<RolesDto> CreateRoleAsync(CreateRoleDto roleDto)
        {
            var newRole = new Role
            {
                Name = roleDto.Name,
                Description = roleDto.Description
            };

            var createdRole = await _roleRepository.AddAsync(newRole);
            return new RolesDto { RoleId = createdRole.RoleId, Name = createdRole.Name, Description = createdRole.Description };
        }

        // ✅ Get a role by ID
        public async Task<RolesDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return null;
            return new RolesDto { RoleId = role.RoleId, Name = role.Name, Description = role.Description };
        }

        // ✅ Update role details
        public async Task<bool> UpdateRoleAsync(int id, CreateRoleDto roleDto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return false;

            role.Name = roleDto.Name;
            role.Description = roleDto.Description;

            return await _roleRepository.UpdateAsync(role);
        }

        // ✅ Delete a role
        public async Task<bool> DeleteRoleAsync(int id)
        {
            return await _roleRepository.DeleteAsync(id);
        }
    }
}
