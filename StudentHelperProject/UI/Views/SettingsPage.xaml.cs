using System.Windows;
using System.Windows.Controls;
using StudentHelper.WPF.UI.Views.Dialogs;

namespace StudentHelper.WPF.UI.Views
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        // ----- ПРОФІЛЬ: зміна імені/прізвища -----
        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditProfileDialog
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                // Оновити відображення імені користувача у верхньому правому куті
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.RefreshUserDisplay();
                }
            }
        }

        // ----- БЕЗПЕКА (зміна пароля) -----
        private void Security_Click(object sender, RoutedEventArgs e)
        {
            // Використаємо вже наявне вікно нового пароля
            var dialog = new NewPasswordWindow
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
        }

        // ----- ПРО ДОДАТОК -----
        private void About_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AboutAppDialog
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
        }

        // ----- ВИЙТИ З АКАУНТУ (було в тебе й раніше) -----
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Ви впевнені, що хочете вийти з акаунту?",
                "Вихід",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Clear user session
                UserSession.Clear();

                var auth = new AuthWindow();
                auth.Show();

                Window.GetWindow(this)?.Close();
            }
        }
    }
}
