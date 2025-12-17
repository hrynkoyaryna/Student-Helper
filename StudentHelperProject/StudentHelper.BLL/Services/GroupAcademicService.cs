using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class GroupAcademicService : IGroupAcademicService
{
    private readonly IGroupAcademicRepository _repo;
    private readonly ILogger<GroupAcademicService> _logger;

    public GroupAcademicService(IGroupAcademicRepository repo, ILogger<GroupAcademicService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<GroupAcademicDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching group with ID {GroupId}", id);
        try
        {
            var g = await _repo.GetByIdAsync(id);
            if (g is null)
            {
                _logger.LogWarning("Group with ID {GroupId} not found", id);
                return null;
            }
            _logger.LogInformation("Group {GroupId} retrieved: {GroupCode}", id, g.Code);
            return MapToDto(g);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching group {GroupId}", id);
            throw;
        }
    }

    public async Task<GroupAcademicDto?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching group with code: {GroupCode}", code);
        try
        {
            var g = await _repo.GetByCodeAsync(code);
            if (g is null)
            {
                _logger.LogWarning("Group with code {GroupCode} not found", code);
                return null;
            }
            _logger.LogInformation("Group {GroupCode} retrieved with ID {GroupId}", code, g.Id);
            return MapToDto(g);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching group by code: {GroupCode}", code);
            throw;
        }
    }

    public async Task<List<GroupAcademicDto>> GetByFacultyAsync(string faculty, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching groups for faculty: {Faculty}", faculty);
        try
        {
            var groups = await _repo.GetGroupsByFacultyAsync(faculty);
            var groupDtos = groups.Select(MapToDto).ToList();
            _logger.LogInformation("Retrieved {GroupCount} groups for faculty {Faculty}", groupDtos.Count, faculty);
            return groupDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching groups for faculty: {Faculty}", faculty);
            throw;
        }
    }

    public async Task<List<GroupAcademicDto>> GetByYearAsync(int year, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching groups for year: {Year}", year);
        try
        {
            var groups = await _repo.GetGroupsByYearAsync(year);
            var groupDtos = groups.Select(MapToDto).ToList();
            _logger.LogInformation("Retrieved {GroupCount} groups for year {Year}", groupDtos.Count, year);
            return groupDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching groups for year: {Year}", year);
            throw;
        }
    }

    private static GroupAcademicDto MapToDto(DAL.Models.GroupAcademic g) =>
        new(g.Id, g.Code, g.Faculty, g.Degree, g.Year);
}
