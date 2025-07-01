using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyManagementAPI.Domain.Entities.OwnerAnnouncements;
using PropertyManagementAPI.Infrastructure.Data;
using System;

namespace PropertyManagementAPI.Infrastructure.Repositories.OwnerAnnouncements
{
    public class OwnerAnnouncementRepository : IOwnerAnnouncementRepository
    {
        private readonly MySqlDbContext _context;

        public OwnerAnnouncementRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OwnerAnnouncement>> GetAllOwnerAsync()
        {
            return await _context.OwnerAnnouncements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.PostedDate)
                .ToListAsync();
        }

        public async Task<OwnerAnnouncement?> GetOwnerByIdAsync(int id)
        {
            return await _context.OwnerAnnouncements.FindAsync(id);
        }

        public async Task<OwnerAnnouncement> CreateOwnerAsync(OwnerAnnouncement announcement)
        {
            _context.OwnerAnnouncements.Add(announcement);
            await _context.SaveChangesAsync();
            return announcement;
        }

        public async Task<bool> UpdateOwnerAsync(OwnerAnnouncement announcement)
        {
            _context.OwnerAnnouncements.Update(announcement);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetActiveOwnerAnnouncementAsync(int announcementId)
        {
            var property = await _context.OwnerAnnouncements.FindAsync(announcementId);
            if (property == null) return false;

            property.IsActive = !property.IsActive;

            var save = await _context.SaveChangesAsync();

            return save > 0;
        }
    }
}
