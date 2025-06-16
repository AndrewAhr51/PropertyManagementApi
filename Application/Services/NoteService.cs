using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Application.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        public async Task<NoteDto> GetNoteByIdAsync(int noteId)
        {
            var note = await _noteRepository.GetNoteByIdAsync(noteId);
            return note != null ? MapToDto(note) : null;
        }

        public async Task<IEnumerable<NoteDto>> GetAllNotesAsync()
        {
            var notes = await _noteRepository.GetAllNotesAsync();
            return notes.Select(MapToDto).ToList();
        }

        public async Task<bool> AddNoteAsync(NoteDto noteDto)
        {
            var note = MapToEntity(noteDto);
            return await _noteRepository.AddNoteAsync(note);
        }

        public async Task<bool> UpdateNoteAsync(NoteDto noteDto)
        {
            var note = MapToEntity(noteDto);
            return await _noteRepository.UpdateNoteAsync(note);
        }

        public async Task<bool> DeleteNoteAsync(int noteId) =>
            await _noteRepository.DeleteNoteAsync(noteId);

        private NoteDto MapToDto(Note note)
        {
            return new NoteDto
            {
                NoteId = note.NoteId,
                CreatedBy = note.CreatedBy,
                TenantId = note.TenantId,
                PropertyId = note.PropertyId,
                NoteText = note.NoteText,
                CreatedAt = note.CreatedAt,
            };
        }

        private Note MapToEntity(NoteDto dto)
        {
            return new Note
            {
                NoteId = dto.NoteId,
                CreatedBy = dto.CreatedBy,
                TenantId = dto.TenantId,
                PropertyId = dto.PropertyId,
                NoteText = dto.NoteText,
                CreatedAt = dto.CreatedAt,
            };
        }
    }
}
