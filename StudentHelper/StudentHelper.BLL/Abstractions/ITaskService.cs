using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Abstractions;

public interface ITaskService
{
    Task<List<TaskDto>> GetUserTasksAsync(int userId, CancellationToken ct = default);
    Task<List<TaskDto>> GetByStatusAsync(int userId, string status, CancellationToken ct = default);

    Task<int> CreateAsync(TaskDto dto, CancellationToken ct = default);
    Task UpdateAsync(TaskDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
