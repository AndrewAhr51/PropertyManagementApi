using PropertyManagementAPI.Domain.Entities.Roles;

namespace PropertyManagementAPI.Infrastructure.Repositories.Roles
{
    public interface IRoleRepository
    {
        Task<Domain.Entities.Roles.Role> AddAsync(Domain.Entities.Roles.Role role);
        Task<Domain.Entities.Roles.Role> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Domain.Entities.Roles.Role role);
        Task<bool> DeleteAsync(int id);
    }
}
