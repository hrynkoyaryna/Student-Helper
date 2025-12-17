using System;
using System.Windows;
using System.Windows.Controls;
using StudentHelper.WPF.UI.ViewModels.Items;

namespace StudentHelper.WPF.UI.Views.Dialogs
{
    public partial class CreateTaskDialog : Window
    {
        public TaskItemViewModel? NewTask { get; private set; }

        public CreateTaskDialog()
        {
            InitializeComponent();

            // Встановити час на наступний день о 16:00
            HourBox.SelectedIndex = 8;  // 16:00
            MinuteBox.SelectedIndex = 0;

            // Реагувати на зміну категорії
            CategoryBox.SelectionChanged += CategoryBox_SelectionChanged;

            // Приховувати поле - вибір можливий тільки для "З предметом"
            UpdateSubjectVisibility();
        }

        private void CategoryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSubjectVisibility();
        }

        private void UpdateSubjectVisibility()
        {
            var category = (CategoryBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            bool isStudy = category == "Навчання";

            // Видимість/сховування поля предмету
            SubjectLabel.Visibility = isStudy ? Visibility.Visible : Visibility.Collapsed;
            SubjectBox.Visibility = isStudy ? Visibility.Visible : Visibility.Collapsed;

            // Встановити текст мітки
            SubjectLabel.Text = isStudy ? "Предмет (для навчання)*" : "Предмет";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введіть назву завдання!", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Введіть дату!", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Отримати час з ComboBox
            string hourStr = (HourBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "16";
            string minuteStr = (MinuteBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "00";

            if (!int.TryParse(hourStr, out int hour) || !int.TryParse(minuteStr, out int minute))
            {
                MessageBox.Show("Невірний час!", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime date = DatePicker.SelectedDate.Value
                .AddHours(hour)
                .AddMinutes(minute);

            var category = (CategoryBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Особисте";

            // Якщо категорія навчання, то помістити предмет
            string subject = category == "Навчання" ? SubjectBox.Text.Trim() : "";

            NewTask = new TaskItemViewModel
            {
                Title = title,
                Description = DescriptionBox.Text.Trim(),
                Category = category,
                Subject = subject,
                DueDate = date,
                IsCompleted = false
            };

            DialogResult = true;
            Close();
        }
    }
}
