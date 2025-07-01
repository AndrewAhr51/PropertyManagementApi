using PropertyManagementAPI.Domain.Entities.OwnerAnnouncements;
using PropertyManagementAPI.Infrastructure.Repositories.OwnerAnnouncements;

namespace PropertyManagementAPI.Application.Services.OwnerAnnouncements
{
    public class OwnerAnnouncementService : IOwnerAnnouncementService
    {
        private readonly IOwnerAnnouncementRepository _repository;

        public OwnerAnnouncementService(IOwnerAnnouncementRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<OwnerAnnouncement>> GetAllOwnerAnnouncementsAsync()
        {
            return await _repository.GetAllOwnerAsync();
        }

        public async Task<OwnerAnnouncement> PostOwnerAnnouncementAsync(string title, string message, string postedBy)
        {
            var newAnnouncement = new OwnerAnnouncement
            {
                Title = title,
                Message = message,
                PostedBy = postedBy
            };

            return await _repository.CreateOwnerAsync(newAnnouncement);
        }

        public async Task<bool> SetActiveOwnerAnnouncementAsync(int id)
        {
            return await _repository.SetActiveOwnerAnnouncementAsync(id);
        }
    }
}
