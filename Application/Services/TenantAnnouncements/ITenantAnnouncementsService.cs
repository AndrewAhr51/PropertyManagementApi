using PropertyManagementAPI.Domain.Entities.TenantAnnouncements;

namespace PropertyManagementAPI.Application.Services.TenantAnnouncements
{
    public interface ITenantAnnouncementService
    {
        Task<IEnumerable<TenantAnnouncement>> GetActiveTenantAnnouncementsAsync();
        Task<TenantAnnouncement> PostTenantAnnouncementAsync(string title, string message, string postedBy);
        Task<bool> SetActiveTenantAnnouncementAsync(int id);
    }
}
