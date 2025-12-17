using System.Text.RegularExpressions;
using System.Windows;
using StudentHelper.BLL.Abstractions;

namespace StudentHelper.WPF.UI.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly IUserService _userService;

        public ForgotPasswordWindow()
        {
            InitializeComponent();
            _userService = ServiceLocator.GetService<IUserService>();
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();

            if (!IsValidEmail(email))
            {
                ErrorText.Text = "*Помилка. Введіть коректну адресу.";
                ErrorText.Visibility = Visibility.Visible;
                EmailTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                return;
            }

            SendButton.IsEnabled = false;
            SendButton.Content = "Відправка...";

            try
            {
                System.Diagnostics.Debug.WriteLine($"[ForgotPassword] Calling SendPasswordResetCodeAsync for: {email}");

                // Send reset code to user
                bool success = await _userService.SendPasswordResetCodeAsync(email);

                System.Diagnostics.Debug.WriteLine($"[ForgotPassword] SendPasswordResetCodeAsync returned: {success}");

                if (!success)
                {
                    ErrorText.Text = "*Користувача з такою адресою не знайдено.";
                    ErrorText.Visibility = Visibility.Visible;
                    EmailTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    SendButton.IsEnabled = true;
                    SendButton.Content = "Відновити пароль";
                    return;
                }

                MessageBox.Show($"На {email} було надіслано код підтвердження. Перевірте папку Spam якщо лист не приходить.",
                    "Відновлення пароля", MessageBoxButton.OK, MessageBoxImage.Information);

                ErrorText.Visibility = Visibility.Collapsed;

                // Перехід до сторінки "Код підтвердження"
                var codeWindow = new ConfirmCodeWindow(email);
                codeWindow.Show();
                Close();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ForgotPassword] Exception: {ex.GetType().Name}: {ex.Message}");
                MessageBox.Show($"Помилка: {ex.Message}\n\nДивіться Debug Output для деталей", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                SendButton.IsEnabled = true;
                SendButton.Content = "Відновити пароль";
            }
        }
    }
}
