using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Authentication
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private string _email = string.Empty;
        private bool _codeSent;
        private string _resetCode = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public bool CodeSent
        {
            get => _codeSent;
            set => SetProperty(ref _codeSent, value);
        }

        public string ResetCode
        {
            get => _resetCode;
            set => SetProperty(ref _resetCode, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public ICommand SendResetCodeCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public ForgotPasswordViewModel(
            IAppService appService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _appService = appService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            SendResetCodeCommand = CreateCommand(async () => await SendResetCodeAsync());
            ResetPasswordCommand = CreateCommand(async () => await ResetPasswordAsync());
            BackToLoginCommand = CreateCommand(async () => await GoBackAsync());

            Title = "Student Helper - Forgot Password";
        }

        private async Task SendResetCodeAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await _dialogService.ShowAlertAsync("Будь ласка, введіть ваш email", "Помилка");
                return;
            }

            try
            {
                IsBusy = true;
                var result = await _appService.SendPasswordResetCodeAsync(Email);

                if (result.IsSuccess)
                {
                    CodeSent = true;
                    await _dialogService.ShowAlertAsync("Код для скидання пароля надіслано на вашу пошту", "Успіх");
                }
                else
                {
                    await _dialogService.ShowAlertAsync(result.ErrorMessage, "Помилка");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка: {ex.Message}", "Помилка");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ResetPasswordAsync()
        {
            if (!ValidateResetData())
                return;

            try
            {
                IsBusy = true;
                // Тимчасова реалізація
                await _dialogService.ShowAlertAsync("Пароль успішно змінено! Тепер ви можете увійти з новим паролем.", "Успіх");
                await _navigationService.NavigateToAsync<LoginViewModel>();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка: {ex.Message}", "Помилка");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool ValidateResetData()
        {
            if (string.IsNullOrWhiteSpace(ResetCode))
            {
                _ = _dialogService.ShowAlertAsync("Будь ласка, введіть код підтвердження", "Помилка");
                return false;
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                _ = _dialogService.ShowAlertAsync("Будь ласка, введіть новий пароль", "Помилка");
                return false;
            }

            if (NewPassword.Length < 8)
            {
                _ = _dialogService.ShowAlertAsync("Пароль повинен містити мінімум 8 символів", "Помилка");
                return false;
            }

            if (NewPassword != ConfirmPassword)
            {
                _ = _dialogService.ShowAlertAsync("Паролі не співпадають", "Помилка");
                return false;
            }

            return true;
        }

        private async Task GoBackAsync()
        {
            await _navigationService.NavigateToAsync<LoginViewModel>();
        }
    }
}