using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using StudentHelper.BLL.Abstractions;

namespace StudentHelper.WPF.UI.ViewModels
{
    public class AuthViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthViewModel> _logger;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isErrorVisible;
        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public AuthViewModel(IUserService userService, ILogger<AuthViewModel> logger)
        {
            _userService = userService;
            _logger = logger;
            _logger.LogInformation("AuthViewModel initialized");
            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin());
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

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set => SetProperty(ref _isErrorVisible, value);
        }

        public ICommand LoginCommand { get; }

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task LoginAsync()
        {
            _logger.LogInformation("Login attempt for email: {Email}", Email);
            IsErrorVisible = false;
            IsLoading = true;
            try
            {
                var user = await _userService.AuthenticateAsync(Email, Password);

                if (user == null)
                {
                    _logger.LogWarning("Failed login attempt: Invalid credentials for email {Email}", Email);
                    ErrorMessage = "Невірний email або пароль";
                    IsErrorVisible = true;
                    return;
                }

                _logger.LogInformation("User {UserId} ({UserEmail}) successfully authenticated", user.Id, user.Email);

                UserSession.CurrentUserId = user.Id;
                UserSession.CurrentUserEmail = user.Email;
                UserSession.CurrentUserName = $"{user.FirstName} {user.LastName}";
                UserSession.CurrentUserFirstName = user.FirstName;
                UserSession.CurrentUserLastName = user.LastName;

                _logger.LogInformation("Opening MainWindow for user {UserId}", user.Id);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = new Views.MainWindow();
                    mainWindow.Show();

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is Views.AuthWindow)
                        {
                            window.Close();
                            break;
                        }
                    }
                });

                _logger.LogInformation("Login successful for user {UserId}", user.Id);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", Email);
                ErrorMessage = $"Помилка входу: {ex.Message}";
                IsErrorVisible = true;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
