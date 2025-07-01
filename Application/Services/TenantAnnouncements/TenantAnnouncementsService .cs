using PropertyManagementAPI.Domain.Entities.TenantAnnouncements;
using PropertyManagementAPI.Infrastructure.Repositories.TenantAnnouncements;

namespace PropertyManagementAPI.Application.Services.TenantAnnouncements
{
    public class TenantAnnouncementService : ITenantAnnouncementService
    {
        private readonly ITenantAnnouncementRepository _repository;

        public TenantAnnouncementService(ITenantAnnouncementRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TenantAnnouncement>> GetActiveTenantAnnouncementsAsync()
        {
            return await _repository.GetAllTenantAsync();
        }

        public async Task<TenantAnnouncement> PostTenantAnnouncementAsync(string title, string message, string postedBy)
        {
            var newAnnouncement = new TenantAnnouncement
            {
                Title = title,
                Message = message,
                PostedBy = postedBy
            };

            return await _repository.CreateTenantAsync(newAnnouncement);
        }

        public async Task<bool> SetActiveTenantAnnouncementAsync(int id)
        {
            return await _repository.SetActiveTenantAnnouncementAsync(id);
        }
    }
}
