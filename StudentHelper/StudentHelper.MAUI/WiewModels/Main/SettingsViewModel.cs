using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;
using StudentHelper.BLL.Abstractions;
using MediatR;
using StudentHelper.BLL.CQRS.Notifications;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.MAUI.ViewModels.Main
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IUserContext _userContext;
        private readonly INotificationSettingService _notificationSettingService;
        private readonly IMediator _mediator;

        private bool _pushNotificationsEnabled = true;
        private bool _telegramConnected = false;
        private bool _googleCalendarSync = false;
        private int _reminderMinutesBefore = 15;

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

        public int ReminderMinutesBefore
        {
            get => _reminderMinutesBefore;
            set => SetProperty(ref _reminderMinutesBefore, value);
        }

        public ICommand SaveSettingsCommand { get; }
        public ICommand ConnectTelegramCommand { get; }
        public ICommand ToggleGoogleCalendarCommand { get; }
        public ICommand LogoutCommand { get; }

        public SettingsViewModel(
            IAppService appService,
            INavigationService navigationService,
            IDialogService dialogService,
            IUserContext userContext,
            INotificationSettingService notificationSettingService,
            IMediator mediator)
        {
            _appService = appService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _userContext = userContext;
            _notificationSettingService = notificationSettingService;
            _mediator = mediator;

            SaveSettingsCommand = CreateCommand(async () => await SaveSettingsAsync());
            ConnectTelegramCommand = CreateCommand(async () => await ConnectTelegramAsync());
            ToggleGoogleCalendarCommand = CreateCommand(async () => await ToggleGoogleCalendarAsync());
            LogoutCommand = CreateCommand(async () => await LogoutAsync());

            Title = "Settings";
        }

        public override async Task InitializeAsync(object? parameter = null)
        {
            await LoadNotificationSettingsAsync();
        }

        private async Task LoadNotificationSettingsAsync()
        {
            try
            {
                IsBusy = true;
                var settings = await _notificationSettingService.GetByUserIdAsync(_userContext.CurrentUserId);
                if (settings != null)
                {
                    TelegramConnected = settings.TelegramEnabled;
                    ReminderMinutesBefore = settings.ReminderMinutesBefore;

                    PushNotificationsEnabled = settings.EmailEnabled;
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка завантаження налаштувань: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveSettingsAsync()
        {
            try
            {
                IsBusy = true;

                await _appService.SaveUserSettingsAsync(new
                {
                    PushNotificationsEnabled,
                    TelegramConnected,
                    GoogleCalendarSync,
                    ReminderMinutesBefore
                });

                var notificationSettingsDto = new NotificationSettingDto(
                    _userContext.CurrentUserId,
                    PushNotificationsEnabled,
                    TelegramConnected,
                    ReminderMinutesBefore
                );

                await _notificationSettingService.UpdateAsync(notificationSettingsDto);

                await _dialogService.ShowAlertAsync("Налаштування збережено", "Успіх");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка збереження: {ex.Message}", "Помилка");
            }
            finally
            {
                IsBusy = false;
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

                    await SaveSettingsAsync();

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

                await SaveSettingsAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка: {ex.Message}", "Помилка");
            }
        }

        private async Task LogoutAsync()
        {
            var confirm = await _dialogService.ShowConfirmationAsync("Ви впевнені, що хочете вийти?");
            if (confirm)
            {
                await _appService.LogoutAsync();
                await _navigationService.NavigateToAsync<Authentication.LoginViewModel>();
            }
        }
    }
}