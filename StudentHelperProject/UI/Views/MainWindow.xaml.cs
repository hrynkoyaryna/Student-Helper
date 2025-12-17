using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace StudentHelper.WPF.UI.Views
{
    public partial class MainWindow : Window
    {
        private Button _currentActiveButton;
        private CalendarPage _calendarPage;
        private TasksPage _tasksPage;
        private ExamsPage _examsPage;
        private NotesPage _notesPage;
        private SettingsPage _settingsPage;

        public MainWindow()
        {
            InitializeComponent();

            // Додаємо обробник для завершення навігації
            MainFrame.Navigated += MainFrame_Navigated;

            // Ініціалізуємо сторінки один раз
            _calendarPage = new CalendarPage();
            _tasksPage = new TasksPage();
            _examsPage = new ExamsPage();
            _notesPage = new NotesPage();
            _settingsPage = new SettingsPage();

            // Встановлюємо календар як активний за замовчуванням
            SetActiveButton(CalendarButton);

            // Відображаємо ім'я користувача
            UpdateUserName();

            // Default — календар з анімацією
            NavigateWithAnimation(_calendarPage);
        }

        private void UpdateUserName()
        {
            if (UserSession.IsAuthenticated)
            {
                // Wenn FirstName und LastName vorhanden sind, verwende sie
                if (!string.IsNullOrWhiteSpace(UserSession.CurrentUserFirstName) &&
                    !string.IsNullOrWhiteSpace(UserSession.CurrentUserLastName))
                {
                    UserNameText.Text = $"{UserSession.CurrentUserFirstName} {UserSession.CurrentUserLastName}";
                }
                // Ansonsten verwende UserName
                else if (!string.IsNullOrWhiteSpace(UserSession.CurrentUserName))
                {
                    UserNameText.Text = UserSession.CurrentUserName;
                }
                else
                {
                    UserNameText.Text = "Користувач";
                }
            }
            else
            {
                UserNameText.Text = "Гість";
            }
        }

        public void RefreshUserDisplay()
        {
            UpdateUserName();
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // Анімація появи нової сторінки
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
            MainFrame.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void NavigateWithAnimation(Page page)
        {
            // Анімація зникнення поточної сторінки
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.2));
            fadeOut.Completed += (s, _) =>
            {
                MainFrame.Navigate(page);
            };
            MainFrame.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void SetActiveButton(Button button)
        {
            // Скидаємо попередню активну кнопку
            if (_currentActiveButton != null)
            {
                _currentActiveButton.Background = new SolidColorBrush(Color.FromArgb(255, 179, 242, 255)); // #B3F2FF
                _currentActiveButton.BorderBrush = Brushes.Black;
            }

            // Встановлюємо нову активну кнопку
            _currentActiveButton = button;
            _currentActiveButton.Background = new SolidColorBrush(Color.FromArgb(255, 154, 221, 234)); // #9ADDEA
            _currentActiveButton.BorderBrush = new SolidColorBrush(Colors.DarkBlue);
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(CalendarButton);
            _calendarPage.RefreshData();
            NavigateWithAnimation(_calendarPage);
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(TasksButton);
            _tasksPage.RefreshData();
            NavigateWithAnimation(_tasksPage);
        }

        private void ExamsButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(ExamsButton);
            _examsPage.RefreshData();
            NavigateWithAnimation(_examsPage);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(SettingsButton);
            NavigateWithAnimation(_settingsPage);
        }

        private void NotesButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(NotesButton);
            _notesPage.RefreshData();
            NavigateWithAnimation(_notesPage);
        }

        private void AddEventTopButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is CalendarPage calendar)
            {
                calendar.AddEventFromDialog();
            }
            else
            {
                MessageBox.Show("Події можна додавати тільки у календарі.");
            }
        }
    }
}