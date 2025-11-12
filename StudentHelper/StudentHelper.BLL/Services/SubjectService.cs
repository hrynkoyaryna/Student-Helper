using DAL.Interfaces;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repo;

    public SubjectService(ISubjectRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<SubjectDto>> GetAllAsync(CancellationToken ct = default)
    {
        var subjects = await _repo.GetAllAsync();
        return subjects.Select(MapToDto).ToList();
    }

    public async Task<SubjectDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var s = await _repo.GetByIdAsync(id);
        return s is null ? null : MapToDto(s);
    }

    public async Task<SubjectDto?> GetByShortNameAsync(string shortName, CancellationToken ct = default)
    {
        var s = await _repo.GetByShortNameAsync(shortName);
        return s is null ? null : MapToDto(s);
    }

    public async Task<List<SubjectDto>> GetByGroupAsync(int groupId, CancellationToken ct = default)
    {
        var subjects = await _repo.GetSubjectsByGroupAsync(groupId);
        return subjects.Select(MapToDto).ToList();
    }

    private static SubjectDto MapToDto(DAL.Models.Subject s) =>
        new(s.Id, s.Name, s.ShortName, s.ColorHex);
}
