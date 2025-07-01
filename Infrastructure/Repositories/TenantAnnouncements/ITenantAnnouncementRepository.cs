using PropertyManagementAPI.Domain.Entities.TenantAnnouncements;

namespace PropertyManagementAPI.Infrastructure.Repositories.TenantAnnouncements
{
    public interface ITenantAnnouncementRepository
    {
        Task<IEnumerable<TenantAnnouncement>> GetAllTenantAsync();
        Task<TenantAnnouncement> GetTenantByIdAsync(int id);
        Task<TenantAnnouncement> CreateTenantAsync(TenantAnnouncement announcement);
        Task<bool> UpdateTenantAsync(TenantAnnouncement announcement);
        Task<bool> SetActiveTenantAnnouncementAsync(int id);
    }
}
