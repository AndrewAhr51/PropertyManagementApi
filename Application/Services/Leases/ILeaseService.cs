using PropertyManagementAPI.Domain.DTOs.Property.Leases;
using PropertyManagementAPI.Domain.Entities.User;

public interface ILeaseService
{
    Task<LeaseDto> CreateLeaseAsync(LeaseDto dto);
    Task<IEnumerable<LeaseDto>> GetAllLeaseAsync();
    Task<LeaseDto?> GetLeaseByIdAsync(int leaseId);
    Task<bool> UpdateLeaseAsync(LeaseUpdateDto dto);
    Task<bool> DeleteLeaseAsync(int leaseId); // Soft delete
    Task<IEnumerable<LeaseDto>> GetAllLeasesByOwnerIdAsync(int ownerId);
    Task<LeaseDto> GetLeaseByTenantIdAsync(int tenantId);
}