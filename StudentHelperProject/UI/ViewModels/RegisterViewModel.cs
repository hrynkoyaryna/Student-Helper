using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StudentHelper.BLL.Abstractions;

namespace StudentHelper.WPF.UI.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _emailErrorMessage = string.Empty;
        private string _passwordErrorMessage = string.Empty;
        private bool _isEmailErrorVisible;
        private bool _isPasswordErrorVisible;
        private bool _isRegistering;

        public RegisterViewModel(IUserService userService)
        {
            _userService = userService;
            RegisterCommand = new RelayCommand(async _ => await RegisterAsync(), _ => CanRegister());
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

        public string EmailErrorMessage
        {
            get => _emailErrorMessage;
            set => SetProperty(ref _emailErrorMessage, value);
        }

        public string PasswordErrorMessage
        {
            get => _passwordErrorMessage;
            set => SetProperty(ref _passwordErrorMessage, value);
        }

        public bool IsEmailErrorVisible
        {
            get => _isEmailErrorVisible;
            set => SetProperty(ref _isEmailErrorVisible, value);
        }

        public bool IsPasswordErrorVisible
        {
            get => _isPasswordErrorVisible;
            set => SetProperty(ref _isPasswordErrorVisible, value);
        }

        public bool IsRegistering
        {
            get => _isRegistering;
            set => SetProperty(ref _isRegistering, value);
        }

        public ICommand RegisterCommand { get; }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   !string.IsNullOrWhiteSpace(FirstName) &&
                   !string.IsNullOrWhiteSpace(LastName) &&
                   !IsRegistering;
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.(edu|edu\.ua)$");
        }

        private bool IsStrongPassword(string pass)
        {
            return pass.Length >= 8 &&
                   Regex.IsMatch(pass, @"[A-Z]") &&
                   Regex.IsMatch(pass, @"\d");
        }

        private async Task RegisterAsync()
        {
            IsEmailErrorVisible = false;
            IsPasswordErrorVisible = false;

            // Email validation
            if (!IsValidEmail(Email))
            {
                EmailErrorMessage = "*Помилка. Введіть адресу в форматі example@univ.edu або example@lnu.edu.ua";
                IsEmailErrorVisible = true;
                return;
            }

            // Password validation
            if (Password != ConfirmPassword)
            {
                PasswordErrorMessage = "*Паролі не збігаються.";
                IsPasswordErrorVisible = true;
                return;
            }

            if (!IsStrongPassword(Password))
            {
                PasswordErrorMessage = "*Пароль занадто простий. Мінімум 8 символів, одна велика, одна цифра.";
                IsPasswordErrorVisible = true;
                return;
            }

            try
            {
                IsRegistering = true;

                await _userService.RegisterUserAsync(
                    Email,
                    Password,
                    FirstName,
                    LastName
                );

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(
                        "Реєстрація успішна! Тепер увійдіть в систему.",
                        "Успіх",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    var authWindow = new Views.AuthWindow();
                    authWindow.Show();

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is Views.RegisterWindow)
                        {
                            window.Close();
                            break;
                        }
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                EmailErrorMessage = "*Користувач з цією адресою вже існує.";
                IsEmailErrorVisible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Помилка реєстрації: {ex.Message}",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                IsRegistering = false;
            }
        }
    }
}
