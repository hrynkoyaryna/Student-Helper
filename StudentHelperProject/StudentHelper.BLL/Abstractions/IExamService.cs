using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Abstractions;

public interface IExamService
{
    Task<List<ExamDto>> GetUserExamsAsync(int userId, CancellationToken ct = default);
    Task<List<ExamDto>> GetUpcomingExamsAsync(int userId, int daysAhead = 30, CancellationToken ct = default);

    Task<int> CreateAsync(ExamDto dto, CancellationToken ct = default);
    Task UpdateAsync(ExamDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
