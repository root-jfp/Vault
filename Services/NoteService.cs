using Microsoft.EntityFrameworkCore;
using Vault.Api.Dtos;
using Vault.Data;
using Vault.Data.Models;

namespace Vault.Services;

public sealed class NoteService(VaultDbContext db)
{
    private const int DefaultUserId = 1;

    public async Task<IReadOnlyList<NoteResponse>> GetAllAsync(string? search = null)
    {
        var query = db.Notes.Where(n => n.UserId == DefaultUserId).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(n => n.Title.Contains(search) || (n.Content != null && n.Content.Contains(search)));

        var notes = await query
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.UpdatedAt)
            .ToListAsync();
        return notes.Select(Map).ToList();
    }

    public async Task<NoteResponse?> GetByIdAsync(int id)
    {
        var n = await db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && n.UserId == DefaultUserId);
        return n is null ? null : Map(n);
    }

    public async Task<NoteResponse> CreateAsync(CreateNoteRequest req)
    {
        var note = new Note
        {
            UserId    = DefaultUserId,
            Title     = req.Title,
            Content   = req.Content,
            Color     = req.Color,
            IsPinned  = req.IsPinned ?? false,
            IsStarred = req.IsStarred ?? false,
            Tags      = req.Tags,
        };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        return Map(note);
    }

    public async Task<NoteResponse?> UpdateAsync(int id, UpdateNoteRequest req)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == DefaultUserId);
        if (note is null) return null;
        if (req.Title     is not null) note.Title     = req.Title;
        if (req.Content   is not null) note.Content   = req.Content;
        if (req.Color     is not null) note.Color     = req.Color;
        if (req.IsPinned  is not null) note.IsPinned  = req.IsPinned.Value;
        if (req.IsStarred is not null) note.IsStarred = req.IsStarred.Value;
        if (req.Tags      is not null) note.Tags      = req.Tags;
        note.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Map(note);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == DefaultUserId);
        if (note is null) return false;
        note.DeletedAt = DateTime.UtcNow;
        note.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<NoteResponse?> TogglePinAsync(int id)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == DefaultUserId);
        if (note is null) return null;
        note.IsPinned  = !note.IsPinned;
        note.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Map(note);
    }

    public async Task<NoteResponse?> ToggleStarAsync(int id)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == DefaultUserId);
        if (note is null) return null;
        note.IsStarred = !note.IsStarred;
        note.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Map(note);
    }

    private static NoteResponse Map(Note n) =>
        new(n.Id, n.Title, n.Content, n.Color, n.IsPinned, n.IsStarred, n.Tags,
            n.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss"));
}
