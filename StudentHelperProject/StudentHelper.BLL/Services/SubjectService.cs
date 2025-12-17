using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class SubjectService : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubjectService> _logger;

    public SubjectService(IUnitOfWork unitOfWork, ILogger<SubjectService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<SubjectDto>> GetAllAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching all subjects");
        try
        {
            var subjects = await _unitOfWork.Subjects.GetAllAsync();
            var subjectDtos = subjects.Select(MapToDto).ToList();
            _logger.LogInformation("Successfully retrieved {SubjectCount} subjects", subjectDtos.Count);
            return subjectDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all subjects");
            throw;
        }
    }

    public async Task<SubjectDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching subject with ID {SubjectId}", id);
        try
        {
            var s = await _unitOfWork.Subjects.GetByIdAsync(id);
            if (s is null)
            {
                _logger.LogWarning("Subject with ID {SubjectId} not found", id);
                return null;
            }
            _logger.LogInformation("Subject {SubjectId} retrieved: {SubjectName}", id, s.Name);
            return MapToDto(s);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching subject {SubjectId}", id);
            throw;
        }
    }

    public async Task<SubjectDto?> GetByShortNameAsync(string shortName, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching subject with short name: {ShortName}", shortName);
        try
        {
            var s = await _unitOfWork.Subjects.GetByShortNameAsync(shortName);
            if (s is null)
            {
                _logger.LogWarning("Subject with short name {ShortName} not found", shortName);
                return null;
            }
            return MapToDto(s);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching subject by short name: {ShortName}", shortName);
            throw;
        }
    }

    public async Task<List<SubjectDto>> GetByGroupAsync(int groupId, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching subjects for group {GroupId}", groupId);
        try
        {
            var subjects = await _unitOfWork.Subjects.GetSubjectsByGroupAsync(groupId);
            var subjectDtos = subjects.Select(MapToDto).ToList();
            _logger.LogInformation("Retrieved {SubjectCount} subjects for group {GroupId}", subjectDtos.Count, groupId);
            return subjectDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching subjects for group {GroupId}", groupId);
            throw;
        }
    }

    public async Task<int> CreateAsync(SubjectDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating new subject: {SubjectName} ({ShortName})", dto.Name, dto.ShortName);
        try
        {
            var subject = new DAL.Models.Subject
            {
                Name = dto.Name,
                ShortName = dto.ShortName,
                Description = string.Empty,
                DefaultColor = dto.ColorHex ?? "#3357FF"
            };

            await _unitOfWork.Subjects.AddAsync(subject);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Subject created successfully with ID {SubjectId}", subject.Id);
            return subject.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating subject: {SubjectName}", dto.Name);
            throw;
        }
    }

    public async Task UpdateAsync(SubjectDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating subject {SubjectId}", dto.Id);
        try
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(dto.Id)
                ?? throw new KeyNotFoundException($"Subject {dto.Id} not found");

            subject.Name = dto.Name;
            subject.ShortName = dto.ShortName;
            subject.DefaultColor = dto.ColorHex ?? subject.DefaultColor;

            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Subject {SubjectId} updated successfully", dto.Id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Subject {SubjectId} not found for update", dto.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating subject {SubjectId}", dto.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting subject {SubjectId}", id);
        try
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Subject {id} not found");

            _unitOfWork.Subjects.Remove(subject);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Subject {SubjectId} deleted successfully", id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Subject {SubjectId} not found for deletion", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting subject {SubjectId}", id);
            throw;
        }
    }

    private static SubjectDto MapToDto(DAL.Models.Subject s) =>
        new(s.Id, s.Name, s.ShortName, s.DefaultColor);
}
