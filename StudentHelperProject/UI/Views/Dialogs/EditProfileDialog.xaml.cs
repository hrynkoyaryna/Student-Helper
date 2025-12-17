using System;
using System.Threading.Tasks;
using System.Windows;
using StudentHelper.BLL.Abstractions;

namespace StudentHelper.WPF.UI.Views.Dialogs
{
    public partial class EditProfileDialog : Window
    {
        private readonly IUserService _userService;

        public string FirstName => FirstNameBox.Text.Trim();
        public string LastName => LastNameBox.Text.Trim();

        public EditProfileDialog()
        {
            InitializeComponent();
            _userService = ServiceLocator.GetService<IUserService>();
            LoadUserData();
        }

        private async void LoadUserData()
        {
            try
            {
                var user = await _userService.GetByIdAsync(UserSession.CurrentUserId);
                if (user != null)
                {
                    FirstNameBox.Text = user.FirstName ?? "";
                    LastNameBox.Text = user.LastName ?? "";
                }
            }
            catch
            {
                // Якщо помилка, залишаємо поля пустими
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstName;
            string lastName = LastName;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля.", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool success = await _userService.UpdateUserProfileAsync(
                    UserSession.CurrentUserId,
                    firstName,
                    lastName);

                if (success)
                {
                    // Оновимо сесію користувача
                    UserSession.CurrentUserFirstName = firstName;
                    UserSession.CurrentUserLastName = lastName;

                    MessageBox.Show("Профіль успішно оновлено.", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Не вдалося оновити профіль.", "Помилка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
