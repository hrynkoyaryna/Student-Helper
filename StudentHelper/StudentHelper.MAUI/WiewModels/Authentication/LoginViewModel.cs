using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.MAUI.ViewModels.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly IUserService _userService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IUserContext _userContext;

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
            IUserService userService,
            INavigationService navigationService,
            IDialogService dialogService,
            IUserContext userContext)
        {
            _appService = appService;
            _userService = userService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _userContext = userContext;

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
                IsBusy = true;
                var result = await _appService.AuthenticateUserAsync(Email, Password);

                if (result.IsSuccess)
                {
                    var user = await _userService.GetByEmailAsync(Email);
                    if (user != null)
                    {
                        _userContext.SetCurrentUser(user.Id);
                        await _navigationService.NavigateToAsync<ViewModels.Main.MainViewModel>();
                    }
                    else
                    {
                        await _dialogService.ShowAlertAsync("Не вдалося отримати дані користувача", "Помилка");
                    }
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
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RegisterAsync()
        {
            await _navigationService.NavigateToAsync<RegisterViewModel>();
        }

        private async Task ForgotPasswordAsync()
        {
            await _navigationService.NavigateToAsync<ForgotPasswordViewModel>();
        }

        private async Task OpenIdLoginAsync()
        {
            try
            {
                IsBusy = true;
                var result = await _appService.AuthenticateWithOpenIdAsync();

                if (result.IsSuccess)
                {
                    // Для OpenID також потрібно отримати реальний ID
                    // Тимчасово залишаємо для тесту, але потім теж виправимо
                    _userContext.SetCurrentUser(1);
                    await _navigationService.NavigateToAsync<ViewModels.Main.MainViewModel>();
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
            finally
            {
                IsBusy = false;
            }
        }
    }
}