using StudentHelper.WPF.UI.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentHelper.WPF.UI.Views.Dialogs
{
    public partial class CreateExamDialog : Window
    {
        public ExamItemViewModel? NewExam { get; private set; }
        private List<string> _availableSubjects = new List<string>();

        public CreateExamDialog()
        {
            InitializeComponent();

            // Додати тестові предмети
            _availableSubjects.Add("Математика");
            _availableSubjects.Add("Фізика");
            _availableSubjects.Add("Програмування");

            FillSubjectComboBox();

            // Встановити час за замовчуванням на 10:00
            HourBox.SelectedIndex = 2;  // 10:00
            MinuteBox.SelectedIndex = 0;
        }

        // Constructor with subjects from parent page
        public CreateExamDialog(List<string> existingSubjects) : this()
        {
            if (existingSubjects != null && existingSubjects.Any())
            {
                _availableSubjects = new List<string>(existingSubjects);
                FillSubjectComboBox();
            }
        }

        private void FillSubjectComboBox()
        {
            SubjectBox.Items.Clear();

            foreach (var subject in _availableSubjects)
            {
                SubjectBox.Items.Add(subject);
            }

            if (SubjectBox.Items.Count > 0)
                SubjectBox.SelectedIndex = 0;
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputDialog("Новий предмет", "Введіть назву предмету:");

            if (inputDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputDialog.Answer))
            {
                string newSubject = inputDialog.Answer.Trim();

                if (!_availableSubjects.Contains(newSubject))
                {
                    _availableSubjects.Add(newSubject);
                    FillSubjectComboBox();
                    SubjectBox.SelectedItem = newSubject;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text) ||
                SubjectBox.SelectedItem == null ||
                TypeBox.SelectedItem == null ||
                DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Будь ласка, заповніть всі обов'язкові поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Отримати час з ComboBox
            string hourStr = (HourBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "10";
            string minuteStr = (MinuteBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "00";

            if (!int.TryParse(hourStr, out int hour) || !int.TryParse(minuteStr, out int minute))
            {
                MessageBox.Show("Некоректний час!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime date = DatePicker.SelectedDate!.Value
                .AddHours(hour)
                .AddMinutes(minute);

            // Get subject as string from ComboBox
            string subject = SubjectBox.SelectedItem as string ?? SubjectBox.SelectedItem?.ToString() ?? "";
            
            if (string.IsNullOrWhiteSpace(subject))
            {
                MessageBox.Show("Будь ласка, оберіть предмет.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Find subject index (1-based for database)
            int subjectIndex = _availableSubjects.IndexOf(subject);
            int subjectId = subjectIndex >= 0 ? subjectIndex + 1 : 1;

            NewExam = new ExamItemViewModel
            {
                Title = TitleBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim(),
                Subject = subject,
                SubjectId = subjectId,
                Date = date,
                IsPassed = false
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
