using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public interface IRoleRepository
    {
        Task<Roles> AddAsync(Roles role);
        Task<Roles> GetByIdAsync(int id);
        Task<bool> UpdateAsync(Roles role);
        Task<bool> DeleteAsync(int id);
    }
}
