using DAL.Interfaces;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class GroupAcademicService : IGroupAcademicService
{
    private readonly IGroupAcademicRepository _repo;

    public GroupAcademicService(IGroupAcademicRepository repo)
    {
        _repo = repo;
    }

    public async Task<GroupAcademicDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var g = await _repo.GetByIdAsync(id);
        return g is null ? null : MapToDto(g);
    }

    public async Task<GroupAcademicDto?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        var g = await _repo.GetByCodeAsync(code);
        return g is null ? null : MapToDto(g);
    }

    public async Task<List<GroupAcademicDto>> GetByFacultyAsync(string faculty, CancellationToken ct = default)
    {
        var groups = await _repo.GetGroupsByFacultyAsync(faculty);
        return groups.Select(MapToDto).ToList();
    }

    public async Task<List<GroupAcademicDto>> GetByYearAsync(int year, CancellationToken ct = default)
    {
        var groups = await _repo.GetGroupsByYearAsync(year);
        return groups.Select(MapToDto).ToList();
    }

    private static GroupAcademicDto MapToDto(DAL.Models.GroupAcademic g) =>
        new(g.Id, g.Code, g.Faculty, g.Degree, g.Year);
}
