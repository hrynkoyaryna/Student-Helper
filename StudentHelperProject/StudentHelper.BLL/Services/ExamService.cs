using DAL.Interfaces;
using DAL.Models;
using Microsoft.Extensions.Logging;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;
using Task = System.Threading.Tasks.Task;

namespace StudentHelper.BLL.Services;

public sealed class ExamService : IExamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExamService> _logger;

    public ExamService(IUnitOfWork unitOfWork, ILogger<ExamService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<ExamDto>> GetUserExamsAsync(int userId, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching all exams for user {UserId}", userId);
        try
        {
            var exams = await _unitOfWork.Exams.GetUserExamsAsync(userId);
            var examDtos = exams.Select(MapToDto).ToList();
            _logger.LogInformation("Successfully retrieved {ExamCount} exams for user {UserId}", examDtos.Count, userId);
            return examDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching exams for user {UserId}", userId);
            throw;
        }
    }

    public async Task<List<ExamDto>> GetUpcomingExamsAsync(int userId, int daysAhead = 30, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching upcoming exams for user {UserId} in the next {DaysAhead} days", userId, daysAhead);
        try
        {
            var exams = await _unitOfWork.Exams.GetUpcomingExamsAsync(userId, daysAhead);
            var examDtos = exams.Select(MapToDto).ToList();
            _logger.LogInformation("Retrieved {UpcomingExamCount} upcoming exams for user {UserId}", examDtos.Count, userId);
            return examDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching upcoming exams for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> CreateAsync(ExamDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating new exam for user {UserId} with title: {Title}, date: {ExamDate}", dto.UserId, dto.Title, dto.ExamDate);
        try
        {
            var e = new Exam
            {
                UserId = dto.UserId,
                SubjectId = dto.SubjectId,
                Title = dto.Title,
                ExamDate = dto.ExamDate,
                StartAt = dto.StartTime,
                EndAt = dto.EndTime,
                Description = dto.Description ?? string.Empty
            };

            await _unitOfWork.Exams.AddAsync(e);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Exam created successfully with ID {ExamId} for user {UserId}", e.Id, dto.UserId);
            return e.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating exam for user {UserId} with title: {Title}", dto.UserId, dto.Title);
            throw;
        }
    }

    public async Task UpdateAsync(ExamDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating exam {ExamId} for user {UserId}", dto.Id, dto.UserId);
        try
        {
            var e = await _unitOfWork.Exams.GetByIdAsync(dto.Id)
                        ?? throw new KeyNotFoundException($"Exam {dto.Id} not found");

            e.UserId = dto.UserId;
            e.SubjectId = dto.SubjectId;
            e.Title = dto.Title;
            e.ExamDate = dto.ExamDate;
            e.StartAt = dto.StartTime;
            e.EndAt = dto.EndTime;
            e.Description = dto.Description ?? string.Empty;

            _unitOfWork.Exams.Update(e);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Exam {ExamId} updated successfully", dto.Id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Exam {ExamId} not found for update", dto.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exam {ExamId} for user {UserId}", dto.Id, dto.UserId);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting exam {ExamId}", id);
        try
        {
            var e = await _unitOfWork.Exams.GetByIdAsync(id)
                        ?? throw new KeyNotFoundException($"Exam {id} not found");

            _unitOfWork.Exams.Remove(e);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Exam {ExamId} deleted successfully", id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Exam {ExamId} not found for deletion", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting exam {ExamId}", id);
            throw;
        }
    }

    private static ExamDto MapToDto(Exam e) =>
        new(
            e.Id,
            e.UserId,
            e.SubjectId,
            e.Title,
            e.ExamDate,
            e.StartAt,
            e.EndAt,
            e.Description
        );
}
