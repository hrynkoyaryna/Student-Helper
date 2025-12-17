using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using StudentHelper.WPF.UI.ViewModels.Items;

namespace StudentHelper.WPF.UI.Views.Dialogs
{
    public partial class CreateEventDialog : Window
    {
        public EventItemViewModel NewEvent { get; private set; }
        private bool _isEditMode = false;

        public CreateEventDialog()
        {
            InitializeComponent();

            // Встановлення початкових значень
            DatePicker.SelectedDate = DateTime.Today;
            StartHourBox.SelectedIndex = 8;   // 08:00
            StartMinuteBox.SelectedIndex = 0;
            EndHourBox.SelectedIndex = 9;     // 09:00
            EndMinuteBox.SelectedIndex = 0;
            TypeBox.SelectedIndex = 0;
            ColorBox.SelectedIndex = 0;
            RecurrenceBox.SelectedIndex = 0;

            // Auto-sync color with type
            TypeBox.SelectionChanged += TypeBox_SelectionChanged;
        }

        public CreateEventDialog(EventItemViewModel existingEvent) : this()
        {
            _isEditMode = true;

            // Change dialog title for edit mode
            Title = "Редагування подієї";

            // Disable recurrence in edit mode
            RecurrenceBox.IsEnabled = false;
            RecurrenceBox.SelectedIndex = 0; // Always "Ніколи"

            // You can add a tooltip or text to explain why it's disabled
            RecurrenceBox.ToolTip = "Повторення можна встановити лише для нових подій При редаганні вже існуючої подієї";
        }

        private void TypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeBox.SelectedItem is ComboBoxItem item && item.Content != null)
            {
                string selectedType = item.Content.ToString() ?? "Нетипізовано";
                Color defaultColor = EventItemViewModel.GetDefaultColorForType(selectedType);

                // Find and select the matching color
                for (int i = 0; i < ColorBox.Items.Count; i++)
                {
                    var colorItem = ColorBox.Items[i] as ComboBoxItem;
                    if (colorItem?.Content is StackPanel stackPanel &&
                        stackPanel.Children[0] is Rectangle rectangle)
                    {
                        var brush = rectangle.Fill as SolidColorBrush;
                        if (brush != null && brush.Color == defaultColor)
                        {
                            ColorBox.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Введіть назву подієї.");
                return;
            }

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Введіть дату.");
                return;
            }

            // Отримати час початку
            string startHourStr = (StartHourBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "08";
            string startMinuteStr = (StartMinuteBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "00";

            // Отримати час завершення
            string endHourStr = (EndHourBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "09";
            string endMinuteStr = (EndMinuteBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "00";

            if (!int.TryParse(startHourStr, out int startHour) ||
                !int.TryParse(startMinuteStr, out int startMinute) ||
                !int.TryParse(endHourStr, out int endHour) ||
                !int.TryParse(endMinuteStr, out int endMinute))
            {
                MessageBox.Show("Некоректний час.");
                return;
            }

            // Отримати дати та часи для розрахування тривалості
            DateTime startDate = DatePicker.SelectedDate.Value
                                 .AddHours(startHour)
                                 .AddMinutes(startMinute);

            DateTime endDate = DatePicker.SelectedDate.Value
                               .AddHours(endHour)
                               .AddMinutes(endMinute);

            if (endDate <= startDate)
            {
                MessageBox.Show("Час завершення має бути пізніше від часу початку.");
                return;
            }

            // Отримати вибраний колір
            Color eventColor = Colors.LightBlue;
            if (ColorBox.SelectedItem is ComboBoxItem colorItem)
            {
                var stackPanel = colorItem.Content as StackPanel;
                if (stackPanel?.Children[0] is Rectangle rectangle)
                {
                    eventColor = ((SolidColorBrush)rectangle.Fill).Color;
                }
            }

            NewEvent = new EventItemViewModel
            {
                Title = TitleBox.Text.Trim(),
                Location = LocationBox.Text.Trim(),
                Description = DescriptionBox.Text.Trim(),
                Type = (TypeBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Нетипізовано",
                StartDate = startDate,
                EndDate = endDate,
                Recurrence = (RecurrenceBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Ніколи",
                EventColor = eventColor
            };

            DialogResult = true;
            Close();
        }
    }
}
