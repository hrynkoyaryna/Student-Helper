using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

/// <summary>
/// Сервіс для управління завданнями.
/// Надає функціонал для створення, отримання, оновлення та видалення завдань користувача.
/// </summary>
public sealed class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TaskService> _logger;

    /// <summary>
    /// Ініціалізує новий екземпляр класу <see cref="TaskService"/>.
    /// </summary>
    /// <param name="unitOfWork">Одиниця роботи для доступу до репозиторіїв.</param>
    /// <param name="logger">Логер для запису подій.</param>
    public TaskService(IUnitOfWork unitOfWork, ILogger<TaskService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Отримує всі завдання користувача.
    /// </summary>
    /// <param name="userId">Ідентифікатор користувача.</param>
    /// <param name="ct">Токен скасування операції.</param>
    /// <returns>Список завдань користувача.</returns>
    public async Task<List<TaskDto>> GetUserTasksAsync(int userId, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching all tasks for user {UserId}", userId);
        try
        {
            var tasks = await _unitOfWork.Tasks.GetUserTasksAsync(userId);
            var taskDtos = tasks.Select(MapToDto).ToList();
            _logger.LogInformation("Successfully retrieved {TaskCount} tasks for user {UserId}", taskDtos.Count, userId);
            return taskDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tasks for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Отримує завдання користувача за статусом.
    /// </summary>
    /// <param name="userId">Ідентифікатор користувача.</param>
    /// <param name="status">Статус завдань для фільтрації:
    /// <list type="bullet">
    /// <item><description>"done" - виконані завдання</description></item>
    /// <item><description>"overdue" - прострочені завдання</description></item>
    /// <item><description>"current" - поточні завдання</description></item>
    /// <item><description>інше - всі завдання</description></item>
    /// </list>
    /// </param>
    /// <param name="ct">Токен скасування операції.</param>
    /// <returns>Відфільтрований список завдань.</returns>
    public async Task<List<TaskDto>> GetByStatusAsync(int userId, string status, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching tasks for user {UserId} with status filter: {Status}", userId, status);
        try
        {
            var allTasks = await _unitOfWork.Tasks.GetUserTasksAsync(userId);
            var now = DateTime.UtcNow;

            var filteredTasks = status switch
            {
                "done" => allTasks.Where(t => t.Status == "done"),
                "overdue" => allTasks.Where(t => t.Status != "done" && t.DueAt < now),
                "current" => allTasks.Where(t => t.Status != "done" && t.DueAt >= now),
                _ => allTasks
            };

            var result = filteredTasks.Select(MapToDto).ToList();
            _logger.LogInformation("Retrieved {FilteredTaskCount} tasks with status '{Status}' for user {UserId}", result.Count, status, userId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching tasks with status {Status} for user {UserId}", status, userId);
            throw;
        }
    }

    /// <summary>
    /// Створює нове завдання.
    /// </summary>
    /// <param name="dto">Об'єкт з даними для створення завдання.</param>
    /// <param name="ct">Токен скасування операції.</param>
    /// <returns>Ідентифікатор створеного завдання.</returns>
    public async Task<int> CreateAsync(TaskDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Creating new task for user {UserId} with title: {Title}", dto.UserId, dto.Title);
        try
        {
            var entity = new DAL.Models.Task
            {
                UserId = dto.UserId,
                SubjectId = dto.SubjectId,
                Title = dto.Title,
                Description = dto.Description,
                DueAt = dto.DueDate,
                Status = dto.Status,
                Priority = dto.Priority,
                Category = dto.Category ?? "Особисте"
            };

            await _unitOfWork.Tasks.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Task created successfully with ID {TaskId} for user {UserId}", entity.Id, dto.UserId);
            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task for user {UserId} with title: {Title}", dto.UserId, dto.Title);
            throw;
        }
    }

    /// <summary>
    /// Оновлює існуюче завдання.
    /// </summary>
    /// <param name="dto">Об'єкт з оновленими даними завдання.</param>
    /// <param name="ct">Токен скасування операції.</param>
    /// <exception cref="KeyNotFoundException">
    /// Викидається, якщо завдання з вказаним ідентифікатором не знайдено.
    /// </exception>
    public async Task UpdateAsync(TaskDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating task {TaskId} for user {UserId}", dto.Id, dto.UserId);
        try
        {
            var entity = await _unitOfWork.Tasks.GetByIdAsync(dto.Id)
                         ?? throw new KeyNotFoundException($"Завдання {dto.Id} не знайдено");

            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.DueAt = dto.DueDate;
            entity.Status = dto.Status;
            entity.Priority = dto.Priority;
            entity.SubjectId = dto.SubjectId;
            entity.Category = dto.Category ?? "Особисте";

            _unitOfWork.Tasks.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Task {TaskId} updated successfully with new status: {Status}", dto.Id, dto.Status);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Task {TaskId} not found for update", dto.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId} for user {UserId}", dto.Id, dto.UserId);
            throw;
        }
    }

    /// <summary>
    /// Видаляє завдання за ідентифікатором.
    /// </summary>
    /// <param name="id">Ідентифікатор завдання.</param>
    /// <param name="ct">Токен скасування операції.</param>
    /// <exception cref="KeyNotFoundException">
    /// Викидається, якщо завдання з вказаним ідентифікатором не знайдено.
    /// </exception>
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        _logger.LogInformation("Deleting task {TaskId}", id);
        try
        {
            var entity = await _unitOfWork.Tasks.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException($"Завдання {id} не знайдено");

            _unitOfWork.Tasks.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Task {TaskId} deleted successfully", id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Task {TaskId} not found for deletion", id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", id);
            throw;
        }
    }

    /// <summary>
    /// Мапить сутність завдання з DAL у DTO.
    /// </summary>
    /// <param name="task">Сутність завдання.</param>
    /// <returns>Об'єкт DTO завдання.</returns>
    private static TaskDto MapToDto(DAL.Models.Task task) =>
        new(
            task.Id,
            task.UserId,
            task.SubjectId,
            task.Title,
            task.Description,
            task.DueAt,
            task.Status ?? "current",
            task.Priority,
            task.Category ?? "Особисте"
        );
}