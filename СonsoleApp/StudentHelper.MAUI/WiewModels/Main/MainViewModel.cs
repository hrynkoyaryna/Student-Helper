using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Main
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAppService _appService;

        public ICommand NavigateToCalendarCommand { get; }
        public ICommand NavigateToTasksCommand { get; }
        public ICommand NavigateToExamsCommand { get; }
        public ICommand NavigateToNotesCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(
            INavigationService navigationService,
            IAppService appService)
        {
            _navigationService = navigationService;
            _appService = appService;

            NavigateToCalendarCommand = CreateCommand(async () => await NavigateToCalendarAsync());
            NavigateToTasksCommand = CreateCommand(async () => await NavigateToTasksAsync());
            NavigateToExamsCommand = CreateCommand(async () => await NavigateToExamsAsync());
            NavigateToNotesCommand = CreateCommand(async () => await NavigateToNotesAsync());
            NavigateToSettingsCommand = CreateCommand(async () => await NavigateToSettingsAsync());
            LogoutCommand = CreateCommand(async () => await LogoutAsync());

            Title = "Student Helper";
        }

        private async Task NavigateToCalendarAsync()
        {
            await _navigationService.NavigateToAsync<CalendarViewModel>();
        }

        private async Task NavigateToTasksAsync()
        {
            await _navigationService.NavigateToAsync<TasksViewModel>();
        }

        private async Task NavigateToExamsAsync()
        {
            await _navigationService.NavigateToAsync<ExamsViewModel>();
        }

        private async Task NavigateToNotesAsync()
        {
            await _navigationService.NavigateToAsync<NotesViewModel>();
        }

        private async Task NavigateToSettingsAsync()
        {
            await _navigationService.NavigateToAsync<SettingsViewModel>();
        }

        private async Task LogoutAsync()
        {
            await _appService.LogoutAsync();
            await _navigationService.NavigateToAsync<Authentication.LoginViewModel>();
        }
    }
}