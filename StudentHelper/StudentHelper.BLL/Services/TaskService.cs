using DAL.Interfaces;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;

    public TaskService(ITaskRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TaskDto>> GetUserTasksAsync(int userId, CancellationToken ct = default)
    {
        var tasks = await _repo.GetUserTasksAsync(userId);
        return tasks.Select(MapToDto).ToList();
    }

    public async Task<List<TaskDto>> GetByStatusAsync(int userId, string status, CancellationToken ct = default)
    {
        var all = await _repo.GetUserTasksAsync(userId);
        var now = DateTime.UtcNow;

        var filtered = status switch
        {
            "done" => all.Where(t => t.Status == "done"),
            "overdue" => all.Where(t => t.Status != "done" && t.DueDate < now),
            "current" => all.Where(t => t.Status != "done" && t.DueDate >= now),
            _ => all
        };

        return filtered.Select(MapToDto).ToList();
    }

    public async Task<int> CreateAsync(TaskDto dto, CancellationToken ct = default)
    {
        var entity = new DAL.Models.Task
        {
            UserId = dto.UserId,
            SubjectId = dto.SubjectId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Status = dto.Status,
            Priority = dto.Priority
        };

        await _repo.AddAsync(entity, ct);
        await _repo.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(TaskDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(dto.Id, ct)
                     ?? throw new KeyNotFoundException($"Task {dto.Id} not found");

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.DueDate = dto.DueDate;
        entity.Status = dto.Status;
        entity.Priority = dto.Priority;
        entity.SubjectId = dto.SubjectId;

        _repo.Update(entity);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
                     ?? throw new KeyNotFoundException($"Task {id} not found");

        _repo.Remove(entity);
        await _repo.SaveChangesAsync(ct);
    }

    private static TaskDto MapToDto(DAL.Models.Task t) =>
        new(
            t.Id,
            t.UserId,
            t.SubjectId,
            t.Title,
            t.Description,
            t.DueDate,
            t.Status ?? "current",
            t.Priority
        );
}
