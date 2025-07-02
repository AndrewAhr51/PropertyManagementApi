using PropertyManagementAPI.Domain.DTOs.Property.Leases;

public interface ILeaseRepository
{
    Task<LeaseDto> CreateLeaseAsync(LeaseDto dto);

    Task<IEnumerable<LeaseDto>> GetAllLeasesAsync();

    Task<LeaseDto?> GetLeaseByIdAsync(int leaseId);

    Task<LeaseDto?> GetLeaseByTenantIdAsync(int tenantId);

    Task<IEnumerable<LeaseDto>> GetAllLeasesByOwnerIdAsync(int ownerId);

    Task<bool> UpdateLeaseByIdAsync(LeaseUpdateDto dto);
    
    Task<bool> DeleteLeaseByIdAsync(int leaseId); // Soft delete
}