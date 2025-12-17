using StudentHelper.WPF.UI.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StudentHelper.WPF.UI.Views.Dialogs
{
    /// <summary>
    /// Dialog for editing existing exams
    /// </summary>
    public partial class EditExamDialog : Window
    {
        public ExamItemViewModel? EditedExam { get; private set; }
        private List<string> _availableSubjects = new List<string>();
        private ExamItemViewModel _originalExam;

        public EditExamDialog(ExamItemViewModel exam, List<string> subjects)
        {
            InitializeComponent();

            _originalExam = exam;
            _availableSubjects = subjects ?? new List<string>();

            FillSubjectComboBox();
            LoadExamData();
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

        private void LoadExamData()
        {
            TitleBox.Text = _originalExam.Title;
            DescriptionBox.Text = _originalExam.Description ?? string.Empty;
            DatePicker.SelectedDate = _originalExam.Date.Date;

            // Select subject
            if (!string.IsNullOrWhiteSpace(_originalExam.Subject))
            {
                for (int i = 0; i < SubjectBox.Items.Count; i++)
                {
                    if (SubjectBox.Items[i].ToString() == _originalExam.Subject)
                    {
                        SubjectBox.SelectedIndex = i;
                        break;
                    }
                }
            }

            // Set time
            int hour = _originalExam.Date.Hour;
            int minute = _originalExam.Date.Minute;

            // Find hour in ComboBox
            for (int i = 0; i < HourBox.Items.Count; i++)
            {
                var item = HourBox.Items[i] as ComboBoxItem;
                if (item != null && int.Parse(item.Content.ToString()) == hour)
                {
                    HourBox.SelectedIndex = i;
                    break;
                }
            }

            // Find minute in ComboBox
            for (int i = 0; i < MinuteBox.Items.Count; i++)
            {
                var item = MinuteBox.Items[i] as ComboBoxItem;
                if (item != null && int.Parse(item.Content.ToString()) == minute)
                {
                    MinuteBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text) ||
                SubjectBox.SelectedItem == null ||
                DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Будь ласка, заповніть усі обов'язкові поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get time
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

            string subject = SubjectBox.SelectedItem as string ?? SubjectBox.SelectedItem?.ToString() ?? "";

            int subjectIndex = _availableSubjects.IndexOf(subject);
            int subjectId = subjectIndex >= 0 ? subjectIndex + 1 : AppConstants.DefaultSubjectId;

            EditedExam = new ExamItemViewModel
            {
                ExamId = _originalExam.ExamId,
                Title = TitleBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim(),
                Subject = subject,
                SubjectId = subjectId,
                Date = date,
                IsPassed = _originalExam.IsPassed
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputDialog("Новий предмет", "Введіть назву предмета:");

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
    }
}
