using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.Application.Services.Owners
{
    public interface IOwnersService
    {
        Task<Owner> AddOwnerAsync(OwnerDto ownerDto);
        Task<IEnumerable<Owner>> GetAllOwnersAsync();
        Task<OwnerDto> GetOwnerByIdAsync(int ownerId);
        Task<OwnerDto> GetOwnerByUserNameAsync(string username);
        Task<OwnerDto> UpdateOwnerAsync(int ownerId, OwnerDto ownerDto);
        Task<bool> DeleteOwnerAsync(int ownerId);
        Task<bool> SetActivateOwnerAsync(int ownerId);

    }
}
