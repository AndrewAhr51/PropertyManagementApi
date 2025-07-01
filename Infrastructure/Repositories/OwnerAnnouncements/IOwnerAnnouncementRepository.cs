using PropertyManagementAPI.Domain.Entities.OwnerAnnouncements;

namespace PropertyManagementAPI.Infrastructure.Repositories.OwnerAnnouncements
{
    public interface IOwnerAnnouncementRepository
    {
        Task<IEnumerable<OwnerAnnouncement>> GetAllOwnerAsync();
        Task<OwnerAnnouncement> GetOwnerByIdAsync(int id);
        Task<OwnerAnnouncement> CreateOwnerAsync(OwnerAnnouncement announcement);
        Task<bool> UpdateOwnerAsync(OwnerAnnouncement announcement);
        Task<bool> SetActiveOwnerAnnouncementAsync(int id);
    }
}
