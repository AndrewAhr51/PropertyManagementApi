using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Notes;
using PropertyManagementAPI.Domain.DTOs.Notes;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet("{noteId}")]
        public async Task<ActionResult<NoteDto>> GetNoteById(int noteId)
        {
            var note = await _noteService.GetNoteByIdAsync(noteId);
            if (note == null) return NotFound();

            return Ok(note);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoteDto>>> GetAllNotes()
        {
            var notes = await _noteService.GetAllNotesAsync();
            return Ok(notes);
        }

        [HttpPost]
        public async Task<ActionResult> AddNote([FromBody] NoteDto noteDto)
        {
            if (noteDto == null) return BadRequest();

            var success = await _noteService.AddNoteAsync(noteDto);
            if (!success) return StatusCode(500, "Error adding note");

            return CreatedAtAction(nameof(GetNoteById), new { noteId = noteDto.NoteId }, noteDto);
        }

        [HttpPut("{noteId}")]
        public async Task<ActionResult> UpdateNote(int noteId, [FromBody] NoteDto noteDto)
        {
            if (noteDto == null || noteId != noteDto.NoteId) return BadRequest();

            var success = await _noteService.UpdateNoteAsync(noteDto);
            if (!success) return StatusCode(500, "Error updating note");

            return NoContent();
        }

        [HttpDelete("{noteId}")]
        public async Task<ActionResult> DeleteNote(int noteId)
        {
            var success = await _noteService.DeleteNoteAsync(noteId);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
