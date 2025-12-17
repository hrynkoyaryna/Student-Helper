using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using StudentHelper.WPF.UI.ViewModels;
using StudentHelper.WPF.UI.ViewModels.Items;
using StudentHelper.WPF.UI.Views.Dialogs;

namespace StudentHelper.WPF.UI.Views
{
    public partial class CalendarPage : Page
    {
        public enum CalendarMode { Day, Week, Month }

        private CalendarMode _currentMode = CalendarMode.Week;
        private DateTime _currentDate = DateTime.Today;

        private readonly List<EventItemViewModel> _events = new();
        private EventItemViewModel? _selectedEvent;
        private Point _dragStartPoint;
        private EventItemViewModel? _draggedEvent;
        private CalendarViewModel? _viewModel;

        public CalendarPage()
        {
            InitializeComponent();

            // Try to get ViewModel from ServiceLocator
            try
            {
                _viewModel = ServiceLocator.GetService<CalendarViewModel>();
                DataContext = _viewModel;

                // Subscribe to ViewModel events changes
                _viewModel.Events.CollectionChanged += (s, e) =>
                {
                    _events.Clear();
                    foreach (var evt in _viewModel.Events)
                    {
                        _events.Add(evt);
                    }
                    Rebuild();
                };
            }
            catch
            {
                // If DI fails, continue with local data
            }

            _currentDate = DateTime.Today;
            _currentMode = CalendarMode.Week;

            WeekButton.IsChecked = true;
            MonthSelector.SelectedIndex = _currentDate.Month - 1;

            CalendarGrid.AllowDrop = true;
            CalendarGrid.Drop += CalendarGrid_Drop;
            CalendarGrid.DragOver += CalendarGrid_DragOver;

            SearchBox.TextChanged += SearchBox_TextChanged;

            SeedDemoEvents();
            Rebuild();
        }

        public void RefreshData()
        {
            // Очистити пошук
            SearchBox.Text = string.Empty;

            // Перезавантажити дані
            if (_viewModel != null)
            {
                _viewModel.LoadEventsCommand.Execute(null);
            }

            // Перебудувати календар
            _currentDate = DateTime.Today;
            Rebuild();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                Rebuild();
                return;
            }

            var filteredEvents = _events.Where(ev =>
                ev.Title.ToLower().Contains(query) ||
                (ev.Location != null && ev.Location.ToLower().Contains(query)) ||
                (ev.Description != null && ev.Description.ToLower().Contains(query))
            ).ToList();

            RebuildWithFilteredEvents(filteredEvents);
        }

        private void FilterCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            bool showPersonal = PersonalFilterCheckBox?.IsChecked == true;
            bool showEducational = EducationalFilterCheckBox?.IsChecked == true;

            var filtered = _events.Where(ev =>
                (showPersonal && ev.Type == "Особиста") ||
                (showEducational && ev.Type == "Навчальна")
            ).ToList();

            if (!showPersonal && !showEducational)
                filtered = _events;

            RebuildWithFilteredEvents(filtered);
        }

        private void RebuildWithFilteredEvents(List<EventItemViewModel> filteredEvents)
        {
            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();

            switch (_currentMode)
            {
                case CalendarMode.Day:
                    GenerateDayGrid();
                    break;
                case CalendarMode.Week:
                    GenerateWeekGrid();
                    break;
                case CalendarMode.Month:
                    GenerateMonthGrid();
                    break;
            }

            RenderFilteredEvents(filteredEvents);
        }

        private void RenderFilteredEvents(List<EventItemViewModel> filteredEvents)
        {
            if (!filteredEvents.Any()) return;

            switch (_currentMode)
            {
                case CalendarMode.Day:
                    RenderDayFilteredEvents(filteredEvents);
                    break;
                case CalendarMode.Week:
                    RenderWeekFilteredEvents(filteredEvents);
                    break;
                case CalendarMode.Month:
                    RenderMonthFilteredEvents(filteredEvents);
                    break;
            }
        }

        private void SeedDemoEvents()
        {
            if (_events.Any()) return;

            _events.Add(new EventItemViewModel
            {
                Title = "Лекція з математики",
                StartDate = DateTime.Today.Date.AddHours(14),
                EndDate = DateTime.Today.Date.AddHours(15).AddMinutes(30),
                Type = "Навчальна",
                Location = "ауд. 215",
                Description = "Прикладова математика",
                EventColor = Colors.LightBlue
            });

            _events.Add(new EventItemViewModel
            {
                Title = "Особиста справа",
                StartDate = DateTime.Today.Date.AddDays(1).AddHours(10),
                EndDate = DateTime.Today.Date.AddDays(1).AddHours(11),
                Type = "Особиста",
                Description = "Запис до лікаря",
                EventColor = Colors.LightGreen
            });

            _events.Add(new EventItemViewModel
            {
                Title = "Семінар з програмування",
                StartDate = DateTime.Today.Date.AddDays(2).AddHours(16),
                EndDate = DateTime.Today.Date.AddDays(2).AddHours(18),
                Type = "Навчальна",
                Location = "ауд. 104",
                Description = "Робота з базами даних",
                EventColor = Colors.LightPink
            });
        }

        private void ViewModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == DayButton)
                _currentMode = CalendarMode.Day;
            else if (sender == WeekButton)
                _currentMode = CalendarMode.Week;
            else
                _currentMode = CalendarMode.Month;

            DayButton.IsChecked = _currentMode == CalendarMode.Day;
            WeekButton.IsChecked = _currentMode == CalendarMode.Week;
            MonthButton.IsChecked = _currentMode == CalendarMode.Month;

            if (_currentMode == CalendarMode.Month)
                MonthSelector.SelectedIndex = _currentDate.Month - 1;

            Rebuild();
        }

        private void MonthSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (MonthSelector.SelectedIndex < 0) return;

            int newMonth = MonthSelector.SelectedIndex + 1;
            var firstOfMonth = new DateTime(_currentDate.Year, newMonth, 1);

            if (_currentMode == CalendarMode.Month)
                _currentDate = firstOfMonth;
            else
                _currentDate = GetFirstMondayOfMonth(firstOfMonth);

            Rebuild();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentMode)
            {
                case CalendarMode.Day:
                    _currentDate = _currentDate.AddDays(-1);
                    break;
                case CalendarMode.Week:
                    _currentDate = _currentDate.AddDays(-7);
                    break;
                case CalendarMode.Month:
                    _currentDate = _currentDate.AddMonths(-1);
                    MonthSelector.SelectedIndex = _currentDate.Month - 1;
                    break;
            }
            Rebuild();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentMode)
            {
                case CalendarMode.Day:
                    _currentDate = _currentDate.AddDays(1);
                    break;
                case CalendarMode.Week:
                    _currentDate = _currentDate.AddDays(7);
                    break;
                case CalendarMode.Month:
                    _currentDate = _currentDate.AddMonths(1);
                    MonthSelector.SelectedIndex = _currentDate.Month - 1;
                    break;
            }
            Rebuild();
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            _currentDate = DateTime.Today;
            if (_currentMode == CalendarMode.Month)
                MonthSelector.SelectedIndex = _currentDate.Month - 1;
            Rebuild();
        }

        private static DateTime GetWeekMonday(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        private static DateTime GetFirstMondayOfMonth(DateTime firstOfMonth)
        {
            int offset = ((int)DayOfWeek.Monday - (int)firstOfMonth.DayOfWeek + 7) % 7;
            return firstOfMonth.AddDays(offset).Date;
        }

        private void Rebuild()
        {
            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();

            switch (_currentMode)
            {
                case CalendarMode.Day:
                    GenerateDayGrid();
                    break;
                case CalendarMode.Week:
                    GenerateWeekGrid();
                    break;
                case CalendarMode.Month:
                    GenerateMonthGrid();
                    break;
            }

            RenderEvents();
        }

        private void GenerateDayGrid()
        {
            // Заголовок з датою
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var dateHeader = new TextBlock
            {
                Text = _currentDate.ToString("dddd, dd MMMM yyyy"),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(dateHeader, 0);
            Grid.SetColumnSpan(dateHeader, 2);
            CalendarGrid.Children.Add(dateHeader);

            // Основна сітка
            CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 8; i <= 20; i++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition());

                var timeCell = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0.5),
                    Child = new TextBlock
                    {
                        Text = $"{i:00}:00",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };
                Grid.SetColumn(timeCell, 0);
                Grid.SetRow(timeCell, i - 7);
                CalendarGrid.Children.Add(timeCell);

                var cell = new Border
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(0.5)
                };
                Grid.SetColumn(cell, 1);
                Grid.SetRow(cell, i - 7);
                CalendarGrid.Children.Add(cell);
            }
        }

        private void GenerateWeekGrid()
        {
            for (int i = 0; i < 8; i++)
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < 14; i++)
                CalendarGrid.RowDefinitions.Add(new RowDefinition());

            DateTime monday = GetWeekMonday(_currentDate);
            string[] dayNames = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Нд" };

            for (int i = 0; i < 7; i++)
            {
                var date = monday.AddDays(i);
                var header = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.5),
                    Background = date.Date == DateTime.Today.Date
                        ? new SolidColorBrush(Color.FromRgb(255, 240, 200))
                        : Brushes.WhiteSmoke,
                    Child = new TextBlock
                    {
                        Text = $"{dayNames[i]} {date:dd.MM}",
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };
                Grid.SetRow(header, 0);
                Grid.SetColumn(header, i + 1);
                CalendarGrid.Children.Add(header);
            }

            for (int h = 8; h <= 20; h++)
            {
                var timeCell = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.5),
                    Child = new TextBlock
                    {
                        Text = $"{h:00}:00",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };
                Grid.SetRow(timeCell, h - 7);
                Grid.SetColumn(timeCell, 0);
                CalendarGrid.Children.Add(timeCell);
            }

            for (int r = 1; r <= 13; r++)
            {
                for (int c = 1; c <= 7; c++)
                {
                    var cell = new Border
                    {
                        BorderBrush = Brushes.LightGray,
                        BorderThickness = new Thickness(0.5)
                    };
                    Grid.SetRow(cell, r);
                    Grid.SetColumn(cell, c);
                    CalendarGrid.Children.Add(cell);
                }
            }
        }

        private void GenerateMonthGrid()
        {
            for (int i = 0; i < 7; i++)
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < 6; i++)
                CalendarGrid.RowDefinitions.Add(new RowDefinition());

            DateTime first = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            int start = ((int)first.DayOfWeek + 6) % 7;
            int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);

            int day = 1;
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    var cell = new Border
                    {
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(0.5),
                        Padding = new Thickness(4),
                        Background = Brushes.White
                    };

                    var panel = new StackPanel();
                    cell.Child = panel;

                    if (r == 0 && c < start)
                    {
                        // пусті клітинки до початку місяця
                    }
                    else if (day <= daysInMonth)
                    {
                        var date = new DateTime(_currentDate.Year, _currentDate.Month, day);
                        panel.Children.Add(new TextBlock
                        {
                            Text = day.ToString(),
                            FontWeight = FontWeights.Bold,
                            Foreground = date.Date == DateTime.Today.Date
                                ? Brushes.DarkOrange
                                : Brushes.Black
                        });
                        day++;
                    }

                    Grid.SetRow(cell, r);
                    Grid.SetColumn(cell, c);
                    CalendarGrid.Children.Add(cell);
                }
            }
        }

        private void RenderEvents()
        {
            if (!_events.Any()) return;

            switch (_currentMode)
            {
                case CalendarMode.Day:
                    RenderDayEvents();
                    break;
                case CalendarMode.Week:
                    RenderWeekEvents();
                    break;
                case CalendarMode.Month:
                    RenderMonthEvents();
                    break;
            }
        }

        private void RenderDayEvents()
        {
            var events = _events.Where(e => e.StartDate.Date == _currentDate.Date).ToList();
            if (!events.Any()) return;

            foreach (var ev in events)
            {
                int startRow = ev.StartDate.Hour - 7;
                int endRow = ev.EndDate.Hour - 7;

                if (startRow < 1) startRow = 1;
                if (endRow > 13) endRow = 13;
                if (startRow > 13) continue;

                var card = CreateEventCard(ev);
                Grid.SetRow(card, startRow);
                Grid.SetColumn(card, 1);
                Grid.SetRowSpan(card, Math.Max(1, endRow - startRow + 1));
                CalendarGrid.Children.Add(card);
            }
        }

        private void RenderDayFilteredEvents(List<EventItemViewModel> events)
        {
            var dayEvents = events.Where(e => e.StartDate.Date == _currentDate.Date).ToList();
            if (!dayEvents.Any()) return;

            foreach (var ev in dayEvents)
            {
                int startRow = ev.StartDate.Hour - 7;
                int endRow = ev.EndDate.Hour - 7;

                if (startRow < 1) startRow = 1;
                if (endRow > 13) endRow = 13;
                if (startRow > 13) continue;

                var card = CreateEventCard(ev);
                Grid.SetRow(card, startRow);
                Grid.SetColumn(card, 1);
                Grid.SetRowSpan(card, Math.Max(1, endRow - startRow + 1));
                CalendarGrid.Children.Add(card);
            }
        }

        private void RenderWeekEvents()
        {
            DateTime monday = GetWeekMonday(_currentDate);
            foreach (var ev in _events)
            {
                if (ev.StartDate.Date < monday || ev.StartDate.Date > monday.AddDays(6)) continue;

                int col = (int)(ev.StartDate.Date - monday).TotalDays + 1;
                int startRow = ev.StartDate.Hour - 7;
                int endRow = ev.EndDate.Hour - 7;

                if (startRow < 1) startRow = 1;
                if (endRow > 13) endRow = 13;
                if (startRow > 13 || col < 1 || col > 7) continue;

                var card = CreateEventCard(ev);
                Grid.SetRow(card, startRow);
                Grid.SetColumn(card, col);
                Grid.SetRowSpan(card, Math.Max(1, endRow - startRow + 1));
                CalendarGrid.Children.Add(card);
            }
        }

        private void RenderWeekFilteredEvents(List<EventItemViewModel> events)
        {
            DateTime monday = GetWeekMonday(_currentDate);
            foreach (var ev in events)
            {
                if (ev.StartDate.Date < monday || ev.StartDate.Date > monday.AddDays(6)) continue;

                int col = (int)(ev.StartDate.Date - monday).TotalDays + 1;
                int startRow = ev.StartDate.Hour - 7;
                int endRow = ev.EndDate.Hour - 7;

                if (startRow < 1) startRow = 1;
                if (endRow > 13) endRow = 13;
                if (startRow > 13 || col < 1 || col > 7) continue;

                var card = CreateEventCard(ev);
                Grid.SetRow(card, startRow);
                Grid.SetColumn(card, col);
                Grid.SetRowSpan(card, Math.Max(1, endRow - startRow + 1));
                CalendarGrid.Children.Add(card);
            }
        }

        private void RenderMonthEvents()
        {
            DateTime first = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            int start = ((int)first.DayOfWeek + 6) % 7;

            foreach (var ev in _events.Where(e =>
                         e.StartDate.Month == _currentDate.Month &&
                         e.StartDate.Year == _currentDate.Year))
            {
                int index = ev.StartDate.Day + start - 1;
                int row = index / 7;
                int col = index % 7;

                var cell = CalendarGrid.Children
                    .OfType<Border>()
                    .FirstOrDefault(b => Grid.GetRow(b) == row && Grid.GetColumn(b) == col);

                if (cell?.Child is StackPanel panel)
                {
                    // Обмежуємо кількість подій на день (щоб не переповнювати)
                    var existingEvents = panel.Children.OfType<Border>().Count();
                    if (existingEvents >= 3) // Максимум 3 події на день
                    {
                        // Якщо це третя подія - показуємо індикатор "+ ще"
                        if (existingEvents == 3)
                        {
                            var moreIndicator = new TextBlock
                            {
                                Text = "+ ще...",
                                FontSize = 9,
                                Foreground = Brushes.Gray,
                                FontStyle = FontStyles.Italic,
                                Margin = new Thickness(2, 1, 2, 1)
                            };
                            panel.Children.Add(moreIndicator);
                        }
                        continue;
                    }

                    panel.Children.Add(CreateMonthEventCard(ev));
                }
            }
        }

        private void RenderMonthFilteredEvents(List<EventItemViewModel> events)
        {
            DateTime first = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            int start = ((int)first.DayOfWeek + 6) % 7;

            foreach (var ev in events.Where(e =>
                         e.StartDate.Month == _currentDate.Month &&
                         e.StartDate.Year == _currentDate.Year))
            {
                int index = ev.StartDate.Day + start - 1;
                int row = index / 7;
                int col = index % 7;

                var cell = CalendarGrid.Children
                    .OfType<Border>()
                    .FirstOrDefault(b => Grid.GetRow(b) == row && Grid.GetColumn(b) == col);

                if (cell?.Child is StackPanel panel)
                {
                    // Обмежуємо кількість подій на день
                    var existingEvents = panel.Children.OfType<Border>().Count();
                    if (existingEvents >= 3)
                    {
                        if (existingEvents == 3)
                        {
                            var moreIndicator = new TextBlock
                            {
                                Text = "+ ще...",
                                FontSize = 9,
                                Foreground = Brushes.Gray,
                                FontStyle = FontStyles.Italic,
                                Margin = new Thickness(2, 1, 2, 1)
                            };
                            panel.Children.Add(moreIndicator);
                        }
                        continue;
                    }

                    panel.Children.Add(CreateMonthEventCard(ev));
                }
            }
        }

        private Border CreateMonthEventCard(EventItemViewModel ev)
        {
            var card = new Border
            {
                Background = new SolidColorBrush(ev.EventColor),
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(0.5),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(2),
                Margin = new Thickness(1),
                Cursor = Cursors.Hand,
                Tag = ev,
                ToolTip = $"{ev.Title}\n{ev.StartDate:HH:mm} - {ev.EndDate:HH:mm}" // Додаємо підказку
            };

            // Обрізаємо текст для компактності
            string displayText = ev.Title.Length > 12
                ? ev.Title.Substring(0, 10) + "..."
                : ev.Title;

            var textBlock = new TextBlock
            {
                Text = displayText,
                FontSize = 9,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = GetContrastColor(ev.EventColor)
            };

            card.Child = textBlock;

            // Додаємо обробники подій
            card.MouseLeftButtonUp += (s, e) =>
            {
                if (s is Border b && b.Tag is EventItemViewModel EventItemViewModel)
                {
                    ShowEventDetails(EventItemViewModel);
                }
            };

            return card;
        }

        // Новий метод для детального перегляду події
        private void ShowEventDetails(EventItemViewModel ev)
        {
            _selectedEvent = ev;

            EventPopupTitleText.Text = ev.Title;
            EventPopupDateText.Text = $"{ev.StartDate:dd.MM.yyyy} {ev.StartDate:HH:mm} - {ev.EndDate:HH:mm}";
            EventPopupLocationText.Text = string.IsNullOrWhiteSpace(ev.Location)
                ? "Місце: —"
                : $"Місце: {ev.Location}";
            EventPopupDescriptionText.Text = string.IsNullOrWhiteSpace(ev.Description)
                ? "Опис відсутній"
                : ev.Description;

            EventPopup.IsOpen = true;
        }

        private Border CreateEventCard(EventItemViewModel ev, bool small = false)
        {
            var card = new Border
            {
                Background = new SolidColorBrush(ev.EventColor),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = small ? new Thickness(2) : new Thickness(4),
                Margin = new Thickness(2),
                Cursor = Cursors.Hand,
                Tag = ev,
                ToolTip = small ? $"{ev.Title}\n{ev.StartDate:HH:mm} - {ev.EndDate:HH:mm}" : null
            };

            string timeText = small ?
                $"{ev.StartDate:HH:mm} {ev.Title}" :
                $"{ev.StartDate:HH:mm}-{ev.EndDate:HH:mm}\n{ev.Title}";

            // Для маленьких карток обрізаємо текст
            if (small && timeText.Length > 20)
            {
                timeText = timeText.Substring(0, 18) + "...";
            }

            card.Child = new TextBlock
            {
                Text = timeText,
                FontWeight = FontWeights.Bold,
                FontSize = small ? 8 : 11,
                TextWrapping = TextWrapping.Wrap,
                Foreground = GetContrastColor(ev.EventColor)
            };

            card.MouseLeftButtonUp += EventCard_Click;
            card.MouseLeftButtonDown += EventCard_MouseDown;
            card.MouseMove += EventCard_MouseMove;

            return card;
        }

        private SolidColorBrush GetContrastColor(Color backgroundColor)
        {
            double luminance = (0.299 * backgroundColor.R + 0.587 * backgroundColor.G + 0.114 * backgroundColor.B) / 255;
            return luminance > 0.5 ? Brushes.Black : Brushes.White;
        }

        private void EventCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border b || b.Tag is not EventItemViewModel ev) return;

            ShowEventDetails(ev);
        }

        private void EventPopup_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent == null) return;

            if (_viewModel != null && _viewModel.DeleteEventCommand.CanExecute(_selectedEvent))
            {
                EventPopup.IsOpen = false;
                _viewModel.DeleteEventCommand.Execute(_selectedEvent);
                _selectedEvent = null;
            }
            else
            {
                var result = MessageBox.Show(
                    $"Ви впевнені, що хочете видалити подію \"{_selectedEvent.Title}\"?\n\nЦю дію неможливо скасувати.",
                    "Видалення події",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _events.Remove(_selectedEvent);
                    _selectedEvent = null;
                    EventPopup.IsOpen = false;
                    Rebuild();

                    MessageBox.Show($"Подію успішно видалено",
                        "Успішно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void EventPopup_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEvent == null) return;

            var dialog = new CreateEventDialog(_selectedEvent)
            {
                Owner = Window.GetWindow(this)
            };

            // Заповнюємо форму існуючими даними
            dialog.TitleBox.Text = _selectedEvent.Title;
            dialog.LocationBox.Text = _selectedEvent.Location ?? "";
            dialog.DescriptionBox.Text = _selectedEvent.Description ?? "";

            // Встановлюємо тип
            foreach (ComboBoxItem item in dialog.TypeBox.Items)
            {
                if (item.Content.ToString() == _selectedEvent.Type)
                {
                    dialog.TypeBox.SelectedItem = item;
                    break;
                }
            }

            // Встановлюємо дату
            dialog.DatePicker.SelectedDate = _selectedEvent.StartDate;

            // Встановлюємо час початку
            foreach (ComboBoxItem item in dialog.StartHourBox.Items)
            {
                if (item.Content.ToString() == _selectedEvent.StartDate.Hour.ToString("00"))
                {
                    dialog.StartHourBox.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in dialog.StartMinuteBox.Items)
            {
                if (item.Content.ToString() == _selectedEvent.StartDate.Minute.ToString("00"))
                {
                    dialog.StartMinuteBox.SelectedItem = item;
                    break;
                }
            }

            // Встановлюємо час закінчення
            foreach (ComboBoxItem item in dialog.EndHourBox.Items)
            {
                if (item.Content.ToString() == _selectedEvent.EndDate.Hour.ToString("00"))
                {
                    dialog.EndHourBox.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in dialog.EndMinuteBox.Items)
            {
                if (item.Content.ToString() == _selectedEvent.EndDate.Minute.ToString("00"))
                {
                    dialog.EndMinuteBox.SelectedItem = item;
                    break;
                }
            }

            // Встановлюємо колір (basierend auf Typ, da Farbe nicht in DB)
            Color typeColor = EventItemViewModel.GetDefaultColorForType(_selectedEvent.Type);
            for (int i = 0; i < dialog.ColorBox.Items.Count; i++)
            {
                var colorItem = dialog.ColorBox.Items[i] as ComboBoxItem;
                if (colorItem?.Content is StackPanel stackPanel &&
                    stackPanel.Children[0] is Rectangle rectangle)
                {
                    var brush = rectangle.Fill as SolidColorBrush;
                    if (brush != null && brush.Color == typeColor)
                    {
                        dialog.ColorBox.SelectedIndex = i;
                        break;
                    }
                }
            }

            // Recurrence is disabled in edit mode, no need to set it

            if (dialog.ShowDialog() == true && dialog.NewEvent != null)
            {
                // Оновлюємо існуючу подію
                _selectedEvent.Title = dialog.NewEvent.Title;
                _selectedEvent.Location = dialog.NewEvent.Location;
                _selectedEvent.Description = dialog.NewEvent.Description;
                _selectedEvent.Type = dialog.NewEvent.Type;
                _selectedEvent.StartDate = dialog.NewEvent.StartDate;
                _selectedEvent.EndDate = dialog.NewEvent.EndDate;
                _selectedEvent.Recurrence = "Ніколи"; // Always "Ніколи" in edit mode
                _selectedEvent.EventColor = dialog.NewEvent.EventColor;

                EventPopup.IsOpen = false;

                if (_viewModel != null && _viewModel.UpdateEventCommand.CanExecute(_selectedEvent))
                {
                    _viewModel.UpdateEventCommand.Execute(_selectedEvent);
                }
                else
                {
                    Rebuild();
                }
            }
        }

        public async void AddEventFromDialog()
        {
            var dialog = new CreateEventDialog();

            if (dialog.ShowDialog() == true && dialog.NewEvent != null)
            {
                if (_viewModel != null)
                {
                    var newEvent = dialog.NewEvent;
                    
                    // Check if recurrence is set
                    if (newEvent.Recurrence != null && newEvent.Recurrence != "Ніколи")
                    {
                        // Ask user how many occurrences they want
                        var result = MessageBox.Show(
                            $"Створити повторювані події?\n\nБуде створено 10 повторень.\n\nТип: {newEvent.Recurrence}",
                            "Повторення події",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);
                            
                        if (result == MessageBoxResult.Yes)
                        {
                            // Create recurring events silently
                            await CreateRecurringEventsAsync(newEvent);
                            return;
                        }
                    }
                    
                    // Single event - add directly
                    if (_viewModel.AddEventCommand.CanExecute(newEvent))
                    {
                        _viewModel.AddEventCommand.Execute(newEvent);
                    }
                }
                else
                {
                    // Fallback to local add
                    _events.Add(dialog.NewEvent);
                    Rebuild();
                }
            }
        }

        private async System.Threading.Tasks.Task CreateRecurringEventsAsync(EventItemViewModel templateEvent)
        {
            try
            {
                var events = GenerateRecurringEvents(templateEvent);
                int successCount = 0;
                
                foreach (var evt in events)
                {
                    try
                    {
                        // Add event silently using the ViewModel's silent method
                        if (_viewModel != null)
                        {
                            bool success = await _viewModel.AddEventSilentlyAsync(evt);
                            if (success)
                            {
                                successCount++;
                            }
                            
                            // Small delay to avoid overwhelming the database
                            await System.Threading.Tasks.Task.Delay(50);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error adding recurring event: {ex.Message}");
                    }
                }
                
                // Reload events to show all new occurrences
                if (_viewModel != null)
                {
                    _viewModel.LoadEventsCommand.Execute(null);
                }
                
                // Show single success message
                MessageBox.Show(
                    $"Успішно створено {successCount} з {events.Count} повторюваних подій",
                    "Успішно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Помилка створення подій: {ex.Message}",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private List<EventItemViewModel> GenerateRecurringEvents(EventItemViewModel templateEvent)
        {
            var events = new List<EventItemViewModel>();
            int occurrences = 10; // Generate 10 occurrences by default
            
            TimeSpan duration = templateEvent.EndDate - templateEvent.StartDate;
            
            for (int i = 0; i < occurrences; i++)
            {
                DateTime eventStart;
                
                switch (templateEvent.Recurrence)
                {
                    case "Щоденно":
                        eventStart = templateEvent.StartDate.AddDays(i);
                        break;
                        
                    case "Щотижня":
                        eventStart = templateEvent.StartDate.AddDays(i * 7);
                        break;
                        
                    case "Щомісяця":
                        eventStart = templateEvent.StartDate.AddMonths(i);
                        break;
                        
                    default:
                        eventStart = templateEvent.StartDate;
                        break;
                }
                
                events.Add(new EventItemViewModel
                {
                    Title = templateEvent.Title,
                    Location = templateEvent.Location,
                    Description = templateEvent.Description,
                    Type = templateEvent.Type,
                    StartDate = eventStart,
                    EndDate = eventStart.Add(duration),
                    Recurrence = "Ніколи", // Individual occurrences don't repeat
                    EventColor = templateEvent.EventColor
                });
            }
            
            return events;
        }

        private void EventCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _dragStartPoint = e.GetPosition(null);
            }
        }

        private void EventCard_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (sender is not Border card || card.Tag is not EventItemViewModel ev) return;

            var position = e.GetPosition(null);
            var diff = position - _dragStartPoint;

            // Only start drag if moved more than 5 pixels
            if (Math.Abs(diff.X) < 5 && Math.Abs(diff.Y) < 5)
                return;

            EventPopup.IsOpen = false;

            _draggedEvent = ev;
            
            try
            {
                DragDrop.DoDragDrop(card, ev, DragDropEffects.Move);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Drag&Drop error: {ex.Message}");
                _draggedEvent = null;
            }
        }

        private void CalendarGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private async void CalendarGrid_Drop(object sender, DragEventArgs e)
        {
            if (_draggedEvent == null) return;

            Point dropPos = e.GetPosition(CalendarGrid);

            int col = -1, row = -1;

            double x = 0;
            for (int c = 0; c < CalendarGrid.ColumnDefinitions.Count; c++)
            {
                double w = CalendarGrid.ColumnDefinitions[c].ActualWidth;
                if (dropPos.X >= x && dropPos.X <= x + w)
                {
                    col = c;
                    break;
                }
                x += w;
            }

            double y = 0;
            for (int r = 0; r < CalendarGrid.RowDefinitions.Count; r++)
            {
                double h = CalendarGrid.RowDefinitions[r].ActualHeight;
                if (dropPos.Y >= y && dropPos.Y <= y + h)
                {
                    row = r;
                    break;
                }
                y += h;
            }

            if (col < 0 || row < 0)
            {
                _draggedEvent = null;
                return;
            }

            DateTime newStartDate = _draggedEvent.StartDate;
            DateTime newEndDate = _draggedEvent.EndDate;
            TimeSpan duration = _draggedEvent.EndDate - _draggedEvent.StartDate;
            bool dateChanged = false;

            if (_currentMode == CalendarMode.Month)
            {
                DateTime first = new DateTime(_currentDate.Year, _currentDate.Month, 1);
                int startDayOffset = ((int)first.DayOfWeek + 6) % 7;
                int cellIndex = row * 7 + col;

                int dayNumber = cellIndex - startDayOffset + 1;

                if (dayNumber >= 1 && dayNumber <= DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month))
                {
                    newStartDate = new DateTime(
                        _currentDate.Year,
                        _currentDate.Month,
                        dayNumber,
                        _draggedEvent.StartDate.Hour,
                        _draggedEvent.StartDate.Minute,
                        0);
                    newEndDate = newStartDate.Add(duration);
                    dateChanged = true;
                }
            }
            else if (_currentMode == CalendarMode.Week)
            {
                DateTime monday = GetWeekMonday(_currentDate);

                if (col >= 1 && col <= 7 && row >= 1 && row <= 13)
                {
                    var day = monday.AddDays(col - 1);
                    int hour = 8 + (row - 1);
                    
                    newStartDate = new DateTime(day.Year, day.Month, day.Day, hour, 0, 0);
                    newEndDate = newStartDate.Add(duration);
                    dateChanged = true;
                }
            }
            else if (_currentMode == CalendarMode.Day)
            {
                if (row >= 2 && row <= 14)
                {
                    int hour = 8 + (row - 2);
                    newStartDate = new DateTime(
                        _currentDate.Year,
                        _currentDate.Month,
                        _currentDate.Day,
                        hour,
                        0,
                        0);
                    newEndDate = newStartDate.Add(duration);
                    dateChanged = true;
                }
            }

            if (!dateChanged)
            {
                _draggedEvent = null;
                return;
            }

            // Update dates locally first for immediate UI feedback
            _draggedEvent.StartDate = newStartDate;
            _draggedEvent.EndDate = newEndDate;
            
            var draggedEventRef = _draggedEvent;
            _draggedEvent = null;
            
            // Rebuild immediately for smooth UX
            Rebuild();
            
            // Update in database asynchronously without blocking UI
            if (_viewModel != null && draggedEventRef.EventId > 0)
            {
                try
                {
                    bool success = await _viewModel.UpdateEventSilentlyAsync(draggedEventRef);
                    
                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine($"Event '{draggedEventRef.Title}' moved successfully");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to update event '{draggedEventRef.Title}'");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating event: {ex.Message}");
                    
                    // Show error only if update fails
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(
                            $"Помилка збереження події: {ex.Message}",
                            "Помилка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    });
                }
            }
        }
    }
}
