using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Abstractions;

public interface ISubjectService
{
    Task<List<SubjectDto>> GetAllAsync(CancellationToken ct = default);
    Task<SubjectDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<SubjectDto?> GetByShortNameAsync(string shortName, CancellationToken ct = default);
    Task<List<SubjectDto>> GetByGroupAsync(int groupId, CancellationToken ct = default);
}
