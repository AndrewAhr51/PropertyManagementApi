using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Application.Services
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
