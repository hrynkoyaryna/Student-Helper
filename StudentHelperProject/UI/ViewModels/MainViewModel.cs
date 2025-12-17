using System.Windows.Input;
using StudentHelper.BLL.Abstractions;

namespace StudentHelper.WPF.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITaskService _taskService;
        private readonly IExamService _examService;
        private readonly INoteService _noteService;
        private readonly ISubjectService _subjectService;
        private readonly INotificationSettingService _notificationService;

        private object? _currentViewModel;

        public MainViewModel(
            ITaskService taskService,
            IExamService examService,
            INoteService noteService,
            ISubjectService subjectService,
            INotificationSettingService notificationService)
        {
            _taskService = taskService;
            _examService = examService;
            _noteService = noteService;
            _subjectService = subjectService;
            _notificationService = notificationService;

            NavigateToCalendarCommand = new RelayCommand(_ => NavigateToCalendar());
            NavigateToTasksCommand = new RelayCommand(_ => NavigateToTasks());
            NavigateToExamsCommand = new RelayCommand(_ => NavigateToExams());
            NavigateToNotesCommand = new RelayCommand(_ => NavigateToNotes());
            NavigateToSettingsCommand = new RelayCommand(_ => NavigateToSettings());
        }

        public object? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public ICommand NavigateToCalendarCommand { get; }
        public ICommand NavigateToTasksCommand { get; }
        public ICommand NavigateToExamsCommand { get; }
        public ICommand NavigateToNotesCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }

        private void NavigateToCalendar()
        {
        }

        private void NavigateToTasks()
        {
        }

        private void NavigateToExams()
        {
        }

        private void NavigateToNotes()
        {
        }

        private void NavigateToSettings()
        {
        }
    }
}
