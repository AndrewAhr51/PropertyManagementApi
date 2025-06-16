using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Application.Services
{
    public interface INoteService
    {
        Task<NoteDto> GetNoteByIdAsync(int noteId);
        Task<IEnumerable<NoteDto>> GetAllNotesAsync();
        Task<bool> AddNoteAsync(NoteDto noteDto);
        Task<bool> UpdateNoteAsync(NoteDto noteDto);
        Task<bool> DeleteNoteAsync(int noteId);
    }
}
