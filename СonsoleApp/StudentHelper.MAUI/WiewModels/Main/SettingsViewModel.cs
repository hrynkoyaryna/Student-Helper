using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Main
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private bool _pushNotificationsEnabled = true;
        private bool _telegramConnected = false;
        private bool _googleCalendarSync = false;

        public bool PushNotificationsEnabled
        {
            get => _pushNotificationsEnabled;
            set => SetProperty(ref _pushNotificationsEnabled, value);
        }

        public bool TelegramConnected
        {
            get => _telegramConnected;
            set => SetProperty(ref _telegramConnected, value);
        }

        public bool GoogleCalendarSync
        {
            get => _googleCalendarSync;
            set => SetProperty(ref _googleCalendarSync, value);
        }

        public ICommand SaveSettingsCommand { get; }
        public ICommand ConnectTelegramCommand { get; }
        public ICommand ToggleGoogleCalendarCommand { get; }

        public SettingsViewModel(
            IAppService appService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _appService = appService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            SaveSettingsCommand = CreateCommand(async () => await SaveSettingsAsync());
            ConnectTelegramCommand = CreateCommand(async () => await ConnectTelegramAsync());
            ToggleGoogleCalendarCommand = CreateCommand(async () => await ToggleGoogleCalendarAsync());

            Title = "Settings";
        }

        private async Task SaveSettingsAsync()
        {
            try
            {
                await _appService.SaveUserSettingsAsync(new
                {
                    PushNotificationsEnabled,
                    TelegramConnected,
                    GoogleCalendarSync
                });

                await _dialogService.ShowAlertAsync("Налаштування збережено", "Успіх");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка збереження: {ex.Message}", "Помилка");
            }
        }

        private async Task ConnectTelegramAsync()
        {
            try
            {
                var result = await _appService.ConnectTelegramAsync();
                if (result.IsSuccess)
                {
                    TelegramConnected = true;
                    await _dialogService.ShowAlertAsync("Telegram успішно підключено", "Успіх");
                }
                else
                {
                    await _dialogService.ShowAlertAsync(result.ErrorMessage, "Помилка");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка підключення: {ex.Message}", "Помилка");
            }
        }

        private async Task ToggleGoogleCalendarAsync()
        {
            try
            {
                if (GoogleCalendarSync)
                {
                    await _appService.DisconnectGoogleCalendarAsync();
                    GoogleCalendarSync = false;
                    await _dialogService.ShowAlertAsync("Google Calendar відключено", "Успіх");
                }
                else
                {
                    var result = await _appService.ConnectGoogleCalendarAsync();
                    if (result.IsSuccess)
                    {
                        GoogleCalendarSync = true;
                        await _dialogService.ShowAlertAsync("Google Calendar успішно підключено", "Успіх");
                    }
                    else
                    {
                        await _dialogService.ShowAlertAsync(result.ErrorMessage, "Помилка");
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка: {ex.Message}", "Помилка");
            }
        }
    }
}