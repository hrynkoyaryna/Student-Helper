using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;
using StudentHelper.WPF.UI.ViewModels.Items;

namespace StudentHelper.WPF.UI.ViewModels
{
    public class TasksViewModel : ViewModelBase
    {
        private readonly ITaskService _taskService;
        private string _selectedTab = "current";
        private string _selectedCategory = "З предметом";
        private bool _isLoading;

        public TasksViewModel(ITaskService taskService)
        {
            _taskService = taskService;

            Tasks = new ObservableCollection<TaskItemViewModel>();

            LoadTasksCommand = new RelayCommand(async _ => await LoadTasksAsync());
            ToggleTaskCommand = new RelayCommand<TaskItemViewModel>(async t => await ToggleTaskAsync(t));
            DeleteTaskCommand = new RelayCommand<TaskItemViewModel>(async t => await DeleteTaskAsync(t));
            AddTaskCommand = new RelayCommand(_ => AddTask());

            // Set default category to show all tasks
            _selectedCategory = "Усі";
        }

        public ObservableCollection<TaskItemViewModel> Tasks { get; }

        public string SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (SetProperty(ref _selectedTab, value))
                {
                    _ = LoadTasksAsync();
                }
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    _ = LoadTasksAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadTasksCommand { get; }
        public ICommand ToggleTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand AddTaskCommand { get; }

        private async Task LoadTasksAsync()
        {
            if (!UserSession.IsAuthenticated) return;

            try
            {
                IsLoading = true;

                var tasks = await _taskService.GetByStatusAsync(UserSession.CurrentUserId, SelectedTab);

                var filtered = tasks.Where(t =>
                    (SelectedCategory == "Усі") ||
                    (SelectedCategory == "Особисте" && t.Category == "Особисте") ||
                    (SelectedCategory == "Навчання" && t.Category == "Навчання")
                ).ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Tasks.Clear();
                    foreach (var task in filtered)
                    {
                        Tasks.Add(MapToViewModel(task));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження завдань: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ToggleTaskAsync(TaskItemViewModel? taskViewModel)
        {
            if (taskViewModel == null) return;

            try
            {
                var newStatus = taskViewModel.IsCompleted ? "current" : "done";

                var dto = new TaskDto(
                    taskViewModel.TaskId,
                    UserSession.CurrentUserId,
                    taskViewModel.SubjectId,
                    taskViewModel.Title,
                    taskViewModel.Description,
                    taskViewModel.DueDate,
                    newStatus,
                    "medium"
                );

                await _taskService.UpdateAsync(dto);
                await LoadTasksAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteTaskAsync(TaskItemViewModel? taskViewModel)
        {
            if (taskViewModel == null) return;

            var result = MessageBox.Show(
                $"Видалити завдання \"{taskViewModel.Title}\"?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _taskService.DeleteAsync(taskViewModel.TaskId);
                    await LoadTasksAsync();
                    MessageBox.Show("Завдання видалено", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddTask()
        {
            var dialog = new Views.Dialogs.CreateTaskDialog
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true && dialog.NewTask != null)
            {
                var dto = new TaskDto(
                    0,
                    UserSession.CurrentUserId,
                    dialog.NewTask.SubjectId,
                    dialog.NewTask.Title,
                    dialog.NewTask.Description,
                    dialog.NewTask.DueDate,
                    "current",
                    "medium"
                );

                Task.Run(async () =>
                {
                    try
                    {
                        await _taskService.CreateAsync(dto);
                        await LoadTasksAsync();

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Завдання створено", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Помилка створення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                });
            }
        }

        private TaskItemViewModel MapToViewModel(TaskDto dto)
        {
            return new TaskItemViewModel
            {
                TaskId = dto.Id,
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                Subject = dto.SubjectId?.ToString() ?? string.Empty,
                SubjectId = dto.SubjectId,
                Category = dto.SubjectId.HasValue ? "З предметом" : "Без предмета",
                DueDate = dto.DueDate ?? DateTime.UtcNow,
                IsCompleted = dto.Status == "done"
            };
        }
    }
}

