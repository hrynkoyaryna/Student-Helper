using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Abstractions;

public interface IGroupAcademicService
{
    Task<GroupAcademicDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<GroupAcademicDto?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<List<GroupAcademicDto>> GetByFacultyAsync(string faculty, CancellationToken ct = default);
    Task<List<GroupAcademicDto>> GetByYearAsync(int year, CancellationToken ct = default);
}
