using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyManagementAPI.Domain.DTOs.TenantAnnouncements;
using PropertyManagementAPI.Domain.Entities.TenantAnnouncements;
using PropertyManagementAPI.Infrastructure.Data;
using System;

namespace PropertyManagementAPI.Infrastructure.Repositories.TenantAnnouncements
{
    public class TenantAnnouncementRepository : ITenantAnnouncementRepository
    {
        private readonly MySqlDbContext _context;

        public TenantAnnouncementRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TenantAnnouncement>> GetAllTenantAsync()
        {
            return await _context.TenantAnnouncements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.PostedDate)
                .ToListAsync();
        }

        public async Task<TenantAnnouncement> GetTenantByIdAsync(int id)
        {
            return await _context.TenantAnnouncements.FindAsync(id);
        }

        public async Task<TenantAnnouncement> CreateTenantAsync(TenantAnnouncement announcement)
        {
            _context.TenantAnnouncements.Add(announcement);
            await _context.SaveChangesAsync();
            return announcement;
        }

        public async Task<bool> UpdateTenantAsync(TenantAnnouncement announcement)
        {
            _context.TenantAnnouncements.Update(announcement);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetActiveTenantAnnouncementAsync(int aanouncementId)
        {
            var property = await _context.TenantAnnouncements.FindAsync(aanouncementId);
            if (property == null) return false;

            property.IsActive = !property.IsActive;

           var save =  await _context.SaveChangesAsync();

            return save > 0;
        }
    }
}
