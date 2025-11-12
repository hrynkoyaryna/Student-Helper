using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Authentication
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public RegisterViewModel(
            IAppService appService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _appService = appService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            RegisterCommand = CreateCommand(async () => await RegisterAsync());
            BackToLoginCommand = CreateCommand(async () => await GoBackAsync());

            Title = "Student Helper - Register";
        }

        private async Task RegisterAsync()
        {
            if (!ValidateInput())
                return;

            try
            {
                IsBusy = true;
                var result = await _appService.RegisterUserAsync(FirstName, LastName, Email, Password);

                if (result.IsSuccess)
                {
                    await _dialogService.ShowAlertAsync("Реєстрація успішна! Будь ласка, увійдіть в систему.", "Успіх");
                    await GoBackAsync();
                }
                else
                {
                    await _dialogService.ShowAlertAsync(result.ErrorMessage, "Помилка реєстрації");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка: {ex.Message}", "Помилка реєстрації");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                _ = _dialogService.ShowAlertAsync("Будь ласка, заповніть всі обов'язкові поля", "Помилка");
                return false;
            }

            if (Password.Length < 8)
            {
                _ = _dialogService.ShowAlertAsync("Пароль повинен містити мінімум 8 символів", "Помилка");
                return false;
            }

            if (Password != ConfirmPassword)
            {
                _ = _dialogService.ShowAlertAsync("Паролі не співпадають", "Помилка");
                return false;
            }

            return true;
        }

        private async Task GoBackAsync()
        {
            await _navigationService.GoBackAsync();
        }
    }
}