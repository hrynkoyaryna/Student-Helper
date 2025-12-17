using DAL.Interfaces;
using DAL.Models;
using Microsoft.Extensions.Logging;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;
using Task = System.Threading.Tasks.Task;

namespace StudentHelper.BLL.Services;

public sealed class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EventService> _logger;

    public EventService(IUnitOfWork unitOfWork, ILogger<EventService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<EventDto>> GetUserEventsAsync(int userId, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching all events for user {UserId}", userId);
        try
        {
            var events = await _unitOfWork.Events.GetUserEventsAsync(userId);
            var eventDtos = events.Select(MapToDto).ToList();
            _logger.LogInformation("Successfully retrieved {EventCount} events for user {UserId}", eventDtos.Count, userId);
            return eventDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching events for user {UserId}", userId);
            throw;
        }
    }

    public async Task<List<EventDto>> GetEventsByDateRangeAsync(int userId, DateTime start, DateTime end, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching events for user {UserId} between {StartDate} and {EndDate}", userId, start, end);
        try
        {
            var events = await _unitOfWork.Events.GetEventsByDateRangeAsync(userId, start, end);
            var eventDtos = events.Select(MapToDto).ToList();
            _logger.LogInformation("Retrieved {EventCount} events in date range for user {UserId}", eventDtos.Count, userId);
            return eventDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching events in date range for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> CreateAsync(EventDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating new event for user {UserId} with title: {Title}", dto.UserId, dto.Title);
        try
        {
            var entity = new Event
            {
                UserId = dto.UserId,
                SubjectId = dto.SubjectId,
                LecturerId = dto.LecturerId,
                RoomId = dto.RoomId,
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Type = dto.EventType,
                RecurrenceRule = dto.RecurrenceRule ?? string.Empty,
                SourceId = dto.SourceId,
                RecurrenceExceptions = string.Empty
            };

            await _unitOfWork.Events.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Event created successfully with ID {EventId} for user {UserId}", entity.Id, dto.UserId);
            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event for user {UserId} with title: {Title}", dto.UserId, dto.Title);
            throw;
        }
    }

    public async Task UpdateAsync(EventDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating event {EventId} for user {UserId}", dto.Id, dto.UserId);
        try
        {
            var entity = await _unitOfWork.Events.GetByIdAsync(dto.Id)
                         ?? throw new KeyNotFoundException($"Event {dto.Id} not found");

            entity.Title = dto.Title;
            entity.Description = dto.Description ?? string.Empty;
            entity.StartAt = dto.StartAt;
            entity.EndAt = dto.EndAt;
            entity.Type = dto.EventType;
            entity.SubjectId = dto.SubjectId;
            entity.LecturerId = dto.LecturerId;
            entity.RoomId = dto.RoomId;
            entity.RecurrenceRule = dto.RecurrenceRule ?? string.Empty;

            _unitOfWork.Events.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Event {EventId} updated successfully", dto.Id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Event {EventId} not found for update", dto.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {EventId} for user {UserId}", dto.Id, dto.UserId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting event {EventId}", id);
        try
        {
            var entity = await _unitOfWork.Events.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Event {id} not found");

            _unitOfWork.Events.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Event {EventId} deleted successfully", id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Event {EventId} not found for deletion", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {EventId}", id);
            throw;
        }
    }

    private static EventDto MapToDto(Event e) =>
        new(
            e.Id,
            e.UserId,
            e.SubjectId,
            e.LecturerId,
            e.RoomId,
            e.Title,
            e.Description,
            e.StartAt,
            e.EndAt ?? e.StartAt.AddHours(1),
            e.Type ?? "personal",
            e.RecurrenceRule,
            e.SourceId
        );
}
