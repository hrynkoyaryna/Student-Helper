using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class NoteService : INoteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NoteService> _logger;

    public NoteService(IUnitOfWork unitOfWork, ILogger<NoteService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<NoteDto>> GetUserNotesAsync(int userId, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching all notes for user {UserId}", userId);
        try
        {
            var notes = await _unitOfWork.Notes.GetUserNotesAsync(userId);
            var noteDtos = notes.Select(MapToDto).ToList();
            _logger.LogInformation("Successfully retrieved {NoteCount} notes for user {UserId}", noteDtos.Count, userId);
            return noteDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching notes for user {UserId}", userId);
            throw;
        }
    }

    public async Task<List<NoteDto>> GetPinnedNotesAsync(int userId, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching pinned notes for user {UserId}", userId);
        try
        {
            var notes = await _unitOfWork.Notes.GetPinnedNotesAsync(userId);
            var noteDtos = notes.Select(MapToDto).ToList();
            _logger.LogInformation("Retrieved {PinnedNoteCount} pinned notes for user {UserId}", noteDtos.Count, userId);
            return noteDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pinned notes for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> CreateAsync(NoteDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating new note for user {UserId} with title: {Title}", dto.UserId, dto.Title);
        try
        {
            var entity = new DAL.Models.Note
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Content = dto.Content ?? string.Empty,
                Body = dto.Content ?? string.Empty,
                IsPinned = dto.IsPinned,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };

            await _unitOfWork.Notes.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Note created successfully with ID {NoteId} for user {UserId}", entity.Id, dto.UserId);
            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note for user {UserId} with title: {Title}", dto.UserId, dto.Title);
            throw;
        }
    }

    public async Task UpdateAsync(NoteDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating note {NoteId} for user {UserId}", dto.Id, dto.UserId);
        try
        {
            var entity = await _unitOfWork.Notes.GetByIdAsync(dto.Id)
                         ?? throw new KeyNotFoundException($"Note {dto.Id} not found");

            entity.Title = dto.Title;
            entity.Content = dto.Content ?? string.Empty;
            entity.Body = dto.Content ?? string.Empty;
            entity.IsPinned = dto.IsPinned;
            entity.UpdatedAt = dto.UpdatedAt;

            _unitOfWork.Notes.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Note {NoteId} updated successfully (Pinned: {IsPinned})", dto.Id, dto.IsPinned);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Note {NoteId} not found for update", dto.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note {NoteId} for user {UserId}", dto.Id, dto.UserId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting note {NoteId}", id);
        try
        {
            var entity = await _unitOfWork.Notes.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Note {id} not found");

            _unitOfWork.Notes.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Note {NoteId} deleted successfully", id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Note {NoteId} not found for deletion", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId}", id);
            throw;
        }
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
