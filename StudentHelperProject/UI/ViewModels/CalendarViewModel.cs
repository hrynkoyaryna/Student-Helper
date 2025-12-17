using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;
using StudentHelper.WPF.UI.ViewModels.Items;

namespace StudentHelper.WPF.UI.ViewModels
{
    public class CalendarViewModel : ViewModelBase
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private DateTime _selectedDate = DateTime.Today;
        private string _viewMode = "Month"; // Day, Week, Month
        private bool _isLoading;

        public CalendarViewModel(IEventService eventService, IUserService userService)
        {
            _eventService = eventService;
            _userService = userService;

            Events = new ObservableCollection<EventItemViewModel>();

            LoadEventsCommand = new RelayCommand(async _ => await LoadEventsAsync());
            AddEventCommand = new RelayCommand<EventItemViewModel>(async e => await AddEventAsync(e));
            UpdateEventCommand = new RelayCommand<EventItemViewModel>(async e => await UpdateEventAsync(e));
            DeleteEventCommand = new RelayCommand<EventItemViewModel>(async e => await DeleteEventAsync(e));
        }

        public ObservableCollection<EventItemViewModel> Events { get; }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    _ = LoadEventsAsync();
                }
            }
        }

        public string ViewMode
        {
            get => _viewMode;
            set
            {
                if (SetProperty(ref _viewMode, value))
                {
                    _ = LoadEventsAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadEventsCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand UpdateEventCommand { get; }
        public ICommand DeleteEventCommand { get; }

        public async System.Threading.Tasks.Task<bool> AddEventSilentlyAsync(EventItemViewModel eventViewModel)
        {
            if (eventViewModel == null || !UserSession.IsAuthenticated)
                return false;

            try
            {
                // Ensure dates are in UTC for database
                DateTime startUtc = eventViewModel.StartDate.Kind == DateTimeKind.Utc
                    ? eventViewModel.StartDate
                    : DateTime.SpecifyKind(eventViewModel.StartDate, DateTimeKind.Utc);

                DateTime endUtc = eventViewModel.EndDate.Kind == DateTimeKind.Utc
                    ? eventViewModel.EndDate
                    : DateTime.SpecifyKind(eventViewModel.EndDate, DateTimeKind.Utc);

                var dto = new EventDto(
                    0,
                    UserSession.CurrentUserId,
                    null,
                    null,
                    null,
                    eventViewModel.Title,
                    eventViewModel.Description,
                    startUtc,
                    endUtc,
                    eventViewModel.Type,
                    null,
                    null
                );

                await _eventService.CreateAsync(dto);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding event: {ex.Message}");
                return false;
            }
        }

        public async System.Threading.Tasks.Task<bool> UpdateEventSilentlyAsync(EventItemViewModel eventViewModel)
        {
            if (eventViewModel == null || eventViewModel.EventId <= 0 || !UserSession.IsAuthenticated)
                return false;

            try
            {
                // Ensure dates are in UTC for database
                DateTime startUtc = eventViewModel.StartDate.Kind == DateTimeKind.Utc
                    ? eventViewModel.StartDate
                    : DateTime.SpecifyKind(eventViewModel.StartDate, DateTimeKind.Utc);

                DateTime endUtc = eventViewModel.EndDate.Kind == DateTimeKind.Utc
                    ? eventViewModel.EndDate
                    : DateTime.SpecifyKind(eventViewModel.EndDate, DateTimeKind.Utc);

                var dto = new EventDto(
                    eventViewModel.EventId,
                    UserSession.CurrentUserId,
                    null,
                    null,
                    null,
                    eventViewModel.Title,
                    eventViewModel.Description,
                    startUtc,
                    endUtc,
                    eventViewModel.Type,
                    null,
                    null
                );

                await _eventService.UpdateAsync(dto);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating event: {ex.Message}");
                return false;
            }
        }

        private async Task LoadEventsAsync()
        {
            try
            {
                IsLoading = true;

                // Calculate date range based on view mode
                DateTime startDate, endDate;
                switch (ViewMode)
                {
                    case "Day":
                        startDate = SelectedDate.Date;
                        endDate = startDate.AddDays(1);
                        break;
                    case "Week":
                        startDate = SelectedDate.Date.AddDays(-(int)SelectedDate.DayOfWeek);
                        endDate = startDate.AddDays(7);
                        break;
                    case "Month":
                    default:
                        startDate = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
                        endDate = startDate.AddMonths(1);
                        break;
                }

                var events = await _eventService.GetEventsByDateRangeAsync(UserSession.CurrentUserId, startDate, endDate);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Events.Clear();
                    foreach (var evt in events)
                    {
                        Events.Add(MapToViewModel(evt));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження подій: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddEvent()
        {
            // TODO: Open dialog to create new event
            MessageBox.Show("Функція додавання нових подій ще не реалізована", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task AddEventAsync(EventItemViewModel? eventViewModel)
        {
            if (eventViewModel == null) return;

            if (!UserSession.IsAuthenticated) return;

            try
            {
                // Ensure dates are in UTC for database
                DateTime startUtc = eventViewModel.StartDate.Kind == DateTimeKind.Utc
                    ? eventViewModel.StartDate
                    : DateTime.SpecifyKind(eventViewModel.StartDate, DateTimeKind.Utc);

                DateTime endUtc = eventViewModel.EndDate.Kind == DateTimeKind.Utc
                    ? eventViewModel.EndDate
                    : DateTime.SpecifyKind(eventViewModel.EndDate, DateTimeKind.Utc);

                var dto = new EventDto(
                    0,
                    UserSession.CurrentUserId,
                    null,
                    null,
                    null,
                    eventViewModel.Title,
                    eventViewModel.Description,
                    startUtc,
                    endUtc,
                    eventViewModel.Type,
                    null,
                    null
                );

                await _eventService.CreateAsync(dto);
                await LoadEventsAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Подія збережена", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task UpdateEventAsync(EventItemViewModel? eventViewModel)
        {
            if (eventViewModel == null || eventViewModel.EventId <= 0) return;

            try
            {
                // Ensure dates are in UTC for database
                DateTime startUtc = eventViewModel.StartDate.Kind == DateTimeKind.Utc
                    ? eventViewModel.StartDate
                    : DateTime.SpecifyKind(eventViewModel.StartDate, DateTimeKind.Utc);

                DateTime endUtc = eventViewModel.EndDate.Kind == DateTimeKind.Utc
                    ? eventViewModel.EndDate
                    : DateTime.SpecifyKind(eventViewModel.EndDate, DateTimeKind.Utc);

                var dto = new EventDto(
                    eventViewModel.EventId,
                    UserSession.CurrentUserId,
                    null,
                    null,
                    null,
                    eventViewModel.Title,
                    eventViewModel.Description,
                    startUtc,
                    endUtc,
                    eventViewModel.Type,
                    null,
                    null
                );

                await _eventService.UpdateAsync(dto);
                await LoadEventsAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Подія оновлена", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private async Task DeleteEventAsync(EventItemViewModel? eventViewModel)
        {
            if (eventViewModel == null) return;

            var result = MessageBox.Show(
                $"Видалити подію \"{eventViewModel.Title}\"?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (eventViewModel.EventId > 0)
                    {
                        await _eventService.DeleteAsync(eventViewModel.EventId);
                    }

                    Events.Remove(eventViewModel);
                    MessageBox.Show("Подія видалена", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private EventItemViewModel MapToViewModel(EventDto dto)
        {
            return new EventItemViewModel
            {
                EventId = dto.Id,
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                StartDate = dto.StartAt.Kind == DateTimeKind.Utc ? dto.StartAt.ToLocalTime() : dto.StartAt,
                EndDate = dto.EndAt.Kind == DateTimeKind.Utc ? dto.EndAt.ToLocalTime() : dto.EndAt,
                Type = dto.EventType,
                Location = string.Empty, // TODO: Map from Room if available
                EventColor = EventItemViewModel.GetDefaultColorForType(dto.EventType)
            };
        }
    }
}
