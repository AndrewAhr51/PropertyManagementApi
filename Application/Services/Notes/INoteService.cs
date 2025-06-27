using PropertyManagementAPI.Domain.DTOs.Notes;

namespace PropertyManagementAPI.Application.Services.Notes
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
