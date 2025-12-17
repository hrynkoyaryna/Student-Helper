using System.Windows;
using StudentHelper.WPF.UI.ViewModels;

namespace StudentHelper.WPF.UI.Views
{
    public partial class AuthWindow : Window
    {
        public AuthWindow() : this(null)
        {
        }

        public AuthWindow(AuthViewModel? viewModel)
        {
            InitializeComponent();
            DataContext = viewModel ?? ServiceLocator.GetService<AuthViewModel>();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string pass = PasswordBox.Password.Trim();

            if (DataContext is AuthViewModel vm)
            {
                vm.Email = email;
                vm.Password = pass;
                
                if (vm.LoginCommand.CanExecute(null))
                {
                    vm.LoginCommand.Execute(null);
                }
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var f = new ForgotPasswordWindow();
            f.Show();
            Close();
        }

        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            var reg = new RegisterWindow();
            reg.Show();
            Close();
        }
    }
}
