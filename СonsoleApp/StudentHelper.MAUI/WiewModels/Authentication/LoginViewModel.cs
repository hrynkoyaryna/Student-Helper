using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private string _email = string.Empty;
        private string _password = string.Empty;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand OpenIdLoginCommand { get; }

        public LoginViewModel(
            IAppService appService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _appService = appService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            LoginCommand = CreateCommand(async () => await LoginAsync());
            RegisterCommand = CreateCommand(async () => await RegisterAsync());
            ForgotPasswordCommand = CreateCommand(async () => await ForgotPasswordAsync());
            OpenIdLoginCommand = CreateCommand(async () => await OpenIdLoginAsync());

            Title = "Student Helper - Login";
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await _dialogService.ShowAlertAsync("Будь ласка, введіть email та пароль");
                return;
            }

            try
            {
                var result = await _appService.AuthenticateUserAsync(Email, Password);

                if (result.IsSuccess)
                {
                    await _navigationService.NavigateToAsync<Main.MainViewModel>();
                }
                else
                {
                    await _dialogService.ShowAlertAsync(result.ErrorMessage, "Помилка входу");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка: {ex.Message}", "Помилка входу");
            }
        }

        private async Task RegisterAsync()
        {
            await _navigationService.NavigateToAsync<RegisterViewModel>();
        }

        private async Task ForgotPasswordAsync()
        {
            // Для тесту просто покажемо повідомлення
            await _dialogService.ShowAlertAsync("Функція відновлення пароля буде реалізована найближчим часом");
        }

        private async Task OpenIdLoginAsync()
        {
            try
            {
                var result = await _appService.AuthenticateWithOpenIdAsync();

                if (result.IsSuccess)
                {
                    await _navigationService.NavigateToAsync<Main.MainViewModel>();
                }
                else
                {
                    await _dialogService.ShowAlertAsync(result.ErrorMessage, "Помилка входу");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка: {ex.Message}", "Помилка входу");
            }
        }
    }
}