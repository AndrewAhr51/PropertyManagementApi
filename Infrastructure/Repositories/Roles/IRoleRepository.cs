using PropertyManagementAPI.Domain.Entities.Roles;

namespace PropertyManagementAPI.Infrastructure.Repositories.Roles
{
    public interface IRoleRepository
    {
        Task<Role> AddAsync(Role role);
        Task<Role> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int id);
    }
}
