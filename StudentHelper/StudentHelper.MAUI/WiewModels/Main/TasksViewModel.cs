using System.Collections.ObjectModel;
using System.Windows.Input;
using MediatR;
using StudentHelper.BLL.CQRS.Tasks;
using StudentHelper.BLL.DTOs;
using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;

namespace StudentHelper.MAUI.ViewModels.Main;

public class TasksViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;
    private readonly IDialogService _dialogService;

    public ObservableCollection<TaskDto> AllTasks { get; } = new();
    public ObservableCollection<TaskDto> CurrentTasks { get; } = new();
    public ObservableCollection<TaskDto> CompletedTasks { get; } = new();

    public ICommand LoadTasksCommand { get; }
    public ICommand CreateTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand ToggleTaskStatusCommand { get; }

    private string _newTaskTitle = string.Empty;
    public string NewTaskTitle
    {
        get => _newTaskTitle;
        set => SetProperty(ref _newTaskTitle, value);
    }

    private string _newTaskDescription = string.Empty;
    public string NewTaskDescription
    {
        get => _newTaskDescription;
        set => SetProperty(ref _newTaskDescription, value);
    }

    private DateTime? _newTaskDueDate = DateTime.Today.AddDays(1);
    public DateTime? NewTaskDueDate
    {
        get => _newTaskDueDate;
        set => SetProperty(ref _newTaskDueDate, value);
    }

    private string _newTaskPriority = "medium";
    public string NewTaskPriority
    {
        get => _newTaskPriority;
        set => SetProperty(ref _newTaskPriority, value);
    }

    public TasksViewModel(IMediator mediator, IUserContext userContext, IDialogService dialogService)
    {
        _mediator = mediator;
        _userContext = userContext;
        _dialogService = dialogService;

        LoadTasksCommand = CreateCommand(async () => await LoadTasksAsync());
        CreateTaskCommand = CreateCommand(async () => await CreateTaskAsync());
        DeleteTaskCommand = CreateCommand<TaskDto>(async (task) => await DeleteTaskAsync(task));
        ToggleTaskStatusCommand = CreateCommand<TaskDto>(async (task) => await ToggleTaskStatusAsync(task));

        Title = "Tasks";
    }

    public override async Task InitializeAsync(object? parameter = null)
    {
        await LoadTasksAsync();
    }

    private async Task LoadTasksAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var allTasksQuery = new GetUserTasksQuery(_userContext.CurrentUserId);
            var allTasks = await _mediator.Send(allTasksQuery);

            var currentTasksQuery = new GetUserTasksByStatusQuery(_userContext.CurrentUserId, "current");
            var currentTasks = await _mediator.Send(currentTasksQuery);

            var completedTasksQuery = new GetUserTasksByStatusQuery(_userContext.CurrentUserId, "completed");
            var completedTasks = await _mediator.Send(completedTasksQuery);

            UpdateCollection(AllTasks, allTasks);
            UpdateCollection(CurrentTasks, currentTasks);
            UpdateCollection(CompletedTasks, completedTasks);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка завантаження завдань: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateCollection(ObservableCollection<TaskDto> collection, List<TaskDto> newItems)
    {
        collection.Clear();
        foreach (var item in newItems)
        {
            collection.Add(item);
        }
    }

    private async Task CreateTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
        {
            await _dialogService.ShowAlertAsync("Будь ласка, введіть назву завдання");
            return;
        }

        try
        {
            var command = new CreateTaskCommand(
                _userContext.CurrentUserId,
                null,
                NewTaskTitle,
                NewTaskDescription,
                NewTaskDueDate,
                NewTaskPriority
            );

            await _mediator.Send(command);

            NewTaskTitle = string.Empty;
            NewTaskDescription = string.Empty;
            NewTaskDueDate = DateTime.Today.AddDays(1);
            NewTaskPriority = "medium";

            await LoadTasksAsync();
            await _dialogService.ShowAlertAsync("Завдання успішно створено");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка створення завдання: {ex.Message}");
        }
    }

    private async Task DeleteTaskAsync(TaskDto task)
    {
        if (task == null) return;

        var confirm = await _dialogService.ShowConfirmationAsync($"Видалити завдання '{task.Title}'?");
        if (!confirm) return;

        try
        {
            var command = new DeleteTaskCommand(task.Id);
            await _mediator.Send(command);
            await LoadTasksAsync();
            await _dialogService.ShowAlertAsync("Завдання успішно видалено");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка видалення завдання: {ex.Message}");
        }
    }

    private async Task ToggleTaskStatusAsync(TaskDto task)
    {
        if (task == null) return;

        try
        {
            var newStatus = task.Status == "completed" ? "current" : "completed";

            var command = new UpdateTaskCommand(
                task.Id,
                task.UserId,
                task.SubjectId,
                task.Title,
                task.Description,
                task.DueDate,
                newStatus,
                task.Priority
            );

            await _mediator.Send(command);
            await LoadTasksAsync();

            var statusText = newStatus == "completed" ? "виконаним" : "поточним";
            await _dialogService.ShowAlertAsync($"Завдання позначено як {statusText}");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка оновлення статусу завдання: {ex.Message}");
        }
    }
}