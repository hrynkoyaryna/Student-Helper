using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Main
{
    public class CalendarViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public ICommand LoadEventsCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand ImportScheduleCommand { get; }

        public CalendarViewModel(
            IAppService appService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _appService = appService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            LoadEventsCommand = CreateCommand(async () => await LoadEventsAsync());
            AddEventCommand = CreateCommand(async () => await AddEventAsync());
            ImportScheduleCommand = CreateCommand(async () => await ImportScheduleAsync());

            Title = "Calendar";
        }

        public override async Task InitializeAsync(object? parameter = null)
        {
            await LoadEventsAsync();
        }

        private async Task LoadEventsAsync()
        {
            try
            {
                var events = await _appService.GetUserEventsAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка завантаження подій: {ex.Message}", "Помилка");
            }
        }

        private async Task AddEventAsync()
        {
            await _dialogService.ShowAlertAsync("Функція додавання події буде реалізована найближчим часом", "Інформація");
        }

        private async Task ImportScheduleAsync()
        {
            try
            {
                var result = await _appService.ImportScheduleAsync();
                if (result.IsSuccess)
                {
                    await _dialogService.ShowAlertAsync("Розклад успішно імпортовано", "Успіх");
                    await LoadEventsAsync();
                }
                else
                {
                    await _dialogService.ShowAlertAsync(result.ErrorMessage, "Помилка імпорту");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка імпорту: {ex.Message}", "Помилка");
            }
        }
    }
}