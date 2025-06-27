using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.Infrastructure.Repositories.Owners
{
    public interface IOwnerRepository
    {
        Task<Owner> AddOwnerAsync(OwnerDto ownerDto);
        Task<IEnumerable<Owner>> GetAllOwnersAsync();
        Task<Owner?> GetOwnerByIdAsync(int ownerId);
        Task<Owner?> GetOwnerByUserNameAsync(string username);
        Task<bool> UpdateOwnerAsync( OwnerDto ownerDto);
        Task<bool> DeleteOwnerAsync(int ownerId);
        Task<bool> SetActivateOwnerAsync(int ownerId); // ✅ New method

    }
}