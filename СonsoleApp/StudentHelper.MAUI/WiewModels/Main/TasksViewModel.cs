using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Main
{
    public class TasksViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public ICommand LoadTasksCommand { get; }
        public ICommand AddTaskCommand { get; }

        public TasksViewModel(
            IAppService appService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _appService = appService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            LoadTasksCommand = CreateCommand(async () => await LoadTasksAsync());
            AddTaskCommand = CreateCommand(async () => await AddTaskAsync());

            Title = "Tasks";
        }

        public override async Task InitializeAsync(object? parameter = null)
        {
            await LoadTasksAsync();
        }

        private async Task LoadTasksAsync()
        {
            try
            {
                var tasks = await _appService.GetUserTasksAsync();
                // Обробка отриманих завдань
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка завантаження завдань: {ex.Message}", "Помилка");
            }
        }

        private async Task AddTaskAsync()
        {
            await _dialogService.ShowAlertAsync("Функція додавання завдання буде реалізована найближчим часом", "Інформація");
        }
    }
}