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

        public ICommand SendResetCodeCommand { get; }
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
        }

        private async Task GoBackAsync()
        {
            await _navigationService.GoBackAsync();
        }
    }
}