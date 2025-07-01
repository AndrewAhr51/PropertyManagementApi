using PropertyManagementAPI.Domain.Entities.OwnerAnnouncements;

namespace PropertyManagementAPI.Application.Services.OwnerAnnouncements
{
    public interface IOwnerAnnouncementService
    {
        Task<IEnumerable<OwnerAnnouncement>> GetAllOwnerAnnouncementsAsync();
        Task<OwnerAnnouncement> PostOwnerAnnouncementAsync(string title, string message, string postedBy);
        Task<bool> SetActiveOwnerAnnouncementAsync(int id);
    }
}
