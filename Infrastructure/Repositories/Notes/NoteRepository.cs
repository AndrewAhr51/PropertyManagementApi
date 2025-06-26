using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Notes
{
    public class NoteRepository : INoteRepository
    {
        private readonly MySqlDbContext _context;

        public NoteRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<Note> GetNoteByIdAsync(int noteId) =>
            await _context.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.NoteId == noteId);

        public async Task<IEnumerable<Note>> GetAllNotesAsync() =>
            await _context.Notes.AsNoTracking().ToListAsync();

        public async Task<bool> AddNoteAsync(Note note)
        {
            await _context.Notes.AddAsync(note);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateNoteAsync(Note note)
        {
            _context.Notes.Update(note);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteNoteAsync(int noteId)
        {
            var note = await GetNoteByIdAsync(noteId);
            if (note == null) return false;

            _context.Notes.Remove(note);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
