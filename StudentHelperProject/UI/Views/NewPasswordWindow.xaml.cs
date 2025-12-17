using System.Windows;
using StudentHelper.BLL.Abstractions;

namespace StudentHelper.WPF.UI.Views
{
    public partial class NewPasswordWindow : Window
    {
        private readonly IUserService _userService;
        private readonly string? _email;
        private readonly bool _isPasswordReset;

        public NewPasswordWindow(string? email = null, bool isPasswordReset = false)
        {
            InitializeComponent();
            _userService = ServiceLocator.GetService<IUserService>();
            _email = email;
            _isPasswordReset = isPasswordReset;

            // If it's a password reset, hide the current password field
            if (_isPasswordReset)
            {
                CurrentPasswordLabel.Visibility = Visibility.Collapsed;
                CurrentPasswordBox.Visibility = Visibility.Collapsed;
                SubmitButton.Content = "Встановити пароль";
            }
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            string p1 = NewPasswordBox.Password.Trim();
            string p2 = ConfirmPasswordBox.Password.Trim();

            // Validate new password
            if (p1.Length < 8)
            {
                ErrorText.Text = "Новий пароль має містити мінімум 8 символів";
                ErrorText.Visibility = Visibility.Visible;
                NewPasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
                return;
            }

            if (p1 != p2)
            {
                ErrorText.Text = "Паролі не співпадають";
                ErrorText.Visibility = Visibility.Visible;
                NewPasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
                ConfirmPasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
                return;
            }

            ErrorText.Visibility = Visibility.Collapsed;
            NewPasswordBox.BorderBrush = System.Windows.Media.Brushes.Gray;
            ConfirmPasswordBox.BorderBrush = System.Windows.Media.Brushes.Gray;

            try
            {
                bool success = false;

                if (_isPasswordReset)
                {
                    // Reset password for email
                    success = await _userService.ResetPasswordAsync(_email, p1);
                    if (success)
                    {
                        MessageBox.Show("Пароль успішно змінено! Будь ласка, увійдіть у систему.", "Готово",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Open login window
                        var loginWindow = new AuthWindow();
                        loginWindow.Show();
                        Close();
                    }
                    else
                    {
                        ErrorText.Text = "Помилка при зміні пароля";
                        ErrorText.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    // Change password for authenticated user
                    if (!UserSession.IsAuthenticated)
                    {
                        MessageBox.Show("Будь ласка, увійдіть в систему", "Помилка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        Close();
                        return;
                    }

                    string currentPassword = CurrentPasswordBox.Password.Trim();
                    success = await _userService.ChangePasswordAsync(
                        UserSession.CurrentUserId,
                        currentPassword,
                        p1);

                    if (success)
                    {
                        MessageBox.Show("Пароль успішно змінено!", "Готово",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        Close();
                    }
                    else
                    {
                        ErrorText.Text = "Поточний пароль невірний";
                        ErrorText.Visibility = Visibility.Visible;
                        CurrentPasswordBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка зміни пароля: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
