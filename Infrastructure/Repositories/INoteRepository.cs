﻿using PropertyManagementAPI.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{
    public interface INoteRepository
    {
        Task<Note> GetNoteByIdAsync(int noteId);
        Task<IEnumerable<Note>> GetAllNotesAsync();
        Task<bool> AddNoteAsync(Note note);
        Task<bool> UpdateNoteAsync(Note note);
        Task<bool> DeleteNoteAsync(int noteId);
    }
}

