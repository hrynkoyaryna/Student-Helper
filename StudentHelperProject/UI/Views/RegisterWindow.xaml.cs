using System.Text.RegularExpressions;
using System.Windows;
using StudentHelper.WPF.UI.ViewModels;

namespace StudentHelper.WPF.UI.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow() : this(null)
        {
        }

        public RegisterWindow(RegisterViewModel? viewModel)
        {
            InitializeComponent();
            DataContext = viewModel ?? ServiceLocator.GetService<RegisterViewModel>();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel vm)
            {
                vm.FirstName = FirstNameBox.Text.Trim();
                vm.LastName = LastNameBox.Text.Trim();
                vm.Email = EmailBox.Text.Trim();
                vm.Password = PasswordBox.Password.Trim();
                vm.ConfirmPassword = ConfirmPasswordBox.Password.Trim();
                
                if (vm.RegisterCommand.CanExecute(null))
                {
                    vm.RegisterCommand.Execute(null);
                }
            }
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new AuthWindow();
            login.Show();
            Close();
        }
    }
}
