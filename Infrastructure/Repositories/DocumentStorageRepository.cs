using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public class DocumentStorageRepository : IDocumentStorageRepository
    {
        private readonly MySqlDbContext _context;

        public DocumentStorageRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentStorage> GetDocumentByIdAsync(int documentStorageId) =>
            await _context.DocumentStorage.AsNoTracking().FirstOrDefaultAsync(d => d.DocumentStorageId == documentStorageId);

        public async Task<IEnumerable<DocumentStorage>> GetAllDocumentsAsync() =>
            await _context.DocumentStorage.AsNoTracking().ToListAsync();

        public async Task<bool> AddDocumentAsync(DocumentStorage documentStorage)
        {
            await _context.DocumentStorage.AddAsync(documentStorage);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteDocumentAsync(int documentStorageId)
        {
            var documentStorage = await GetDocumentByIdAsync(documentStorageId);
            if (documentStorage == null) return false;

            _context.DocumentStorage.Remove(documentStorage);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}