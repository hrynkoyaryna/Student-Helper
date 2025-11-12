using DAL.Interfaces;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class NoteService : INoteService
{
    private readonly INoteRepository _repo;

    public NoteService(INoteRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<NoteDto>> GetUserNotesAsync(int userId, CancellationToken ct = default)
    {
        var notes = await _repo.GetUserNotesAsync(userId);
        return notes.Select(MapToDto).ToList();
    }

    public async Task<List<NoteDto>> GetPinnedNotesAsync(int userId, CancellationToken ct = default)
    {
        var notes = await _repo.GetPinnedNotesAsync(userId);
        return notes.Select(MapToDto).ToList();
    }

    public async Task<int> CreateAsync(NoteDto dto, CancellationToken ct = default)
    {
        var entity = new DAL.Models.Note
        {
            UserId = dto.UserId,
            Title = dto.Title,
            Content = dto.Content,
            Body = dto.Content,
            IsPinned = dto.IsPinned
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(NoteDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(dto.Id, ct)
                     ?? throw new KeyNotFoundException($"Note {dto.Id} not found");

        entity.Title = dto.Title;
        entity.Content = dto.Content;
        entity.Body = dto.Content;
        entity.IsPinned = dto.IsPinned;

        _repo.Update(entity);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException($"Note {id} not found");

        _repo.Remove(entity);
        await _repo.SaveChangesAsync(ct);
    }

    private static NoteDto MapToDto(DAL.Models.Note n) =>
        new(
            n.Id,
            n.UserId,
            n.Title,
            n.Content ?? n.Body,
            n.IsPinned,
            n.CreatedAt,
            n.UpdatedAt
        );
}
