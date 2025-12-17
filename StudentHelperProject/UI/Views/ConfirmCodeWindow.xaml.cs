using System.Windows;
using StudentHelper.BLL.Abstractions;

namespace StudentHelper.WPF.UI.Views
{
    public partial class ConfirmCodeWindow : Window
    {
        private readonly IUserService _userService;
        private readonly string _email;

        public ConfirmCodeWindow(string email)
        {
            InitializeComponent();
            _email = email;
            _userService = ServiceLocator.GetService<IUserService>();
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string code = CodeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(code))
            {
                ErrorText.Visibility = Visibility.Visible;
                CodeTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                return;
            }

            try
            {
                // Verify code
                bool isValid = await _userService.VerifyPasswordResetCodeAsync(_email, code);

                if (!isValid)
                {
                    ErrorText.Visibility = Visibility.Visible;
                    CodeTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                    return;
                }

                ErrorText.Visibility = Visibility.Collapsed;

                // Перехід до нового пароля
                var newPassWindow = new NewPasswordWindow(_email, isPasswordReset: true);
                newPassWindow.Show();
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
