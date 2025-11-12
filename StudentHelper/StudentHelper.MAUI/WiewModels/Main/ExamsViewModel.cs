using System.Collections.ObjectModel;
using System.Windows.Input;
using MediatR;
using StudentHelper.BLL.CQRS.Exams;
using StudentHelper.BLL.DTOs;
using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using StudentHelper.BLL.CQRS.Subjects;
using StudentHelper.BLL.Abstractions;

namespace StudentHelper.MAUI.ViewModels.Main;

public class ExamsViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;
    private readonly IDialogService _dialogService;
    private readonly ISubjectService _subjectService;

    public ObservableCollection<ExamDto> AllExams { get; } = new();
    public ObservableCollection<ExamDto> UpcomingExams { get; } = new();
    public ObservableCollection<SubjectDto> Subjects { get; } = new();

    public ICommand LoadExamsCommand { get; }
    public ICommand LoadUpcomingExamsCommand { get; }
    public ICommand CreateExamCommand { get; }
    public ICommand DeleteExamCommand { get; }
    public ICommand LoadSubjectsCommand { get; }
    private string _newExamTitle = string.Empty;
    public string NewExamTitle
    {
        get => _newExamTitle;
        set => SetProperty(ref _newExamTitle, value);
    }

    private DateTime _newExamDate = DateTime.Today.AddDays(7);
    public DateTime NewExamDate
    {
        get => _newExamDate;
        set => SetProperty(ref _newExamDate, value);
    }

    private TimeSpan? _newExamStartTime = new TimeSpan(9, 0, 0);
    public TimeSpan? NewExamStartTime
    {
        get => _newExamStartTime;
        set => SetProperty(ref _newExamStartTime, value);
    }

    private TimeSpan? _newExamEndTime = new TimeSpan(11, 0, 0);
    public TimeSpan? NewExamEndTime
    {
        get => _newExamEndTime;
        set => SetProperty(ref _newExamEndTime, value);
    }

    private string _newExamDescription = string.Empty;
    public string NewExamDescription
    {
        get => _newExamDescription;
        set => SetProperty(ref _newExamDescription, value);
    }

    private int _selectedSubjectId;
    public int SelectedSubjectId
    {
        get => _selectedSubjectId;
        set => SetProperty(ref _selectedSubjectId, value);
    }

    public ExamsViewModel(
        IMediator mediator,
        IUserContext userContext,
        IDialogService dialogService,
        ISubjectService subjectService)
    {
        _mediator = mediator;
        _userContext = userContext;
        _dialogService = dialogService;
        _subjectService = subjectService;

        LoadExamsCommand = CreateCommand(async () => await LoadExamsAsync());
        LoadUpcomingExamsCommand = CreateCommand(async () => await LoadUpcomingExamsAsync());
        CreateExamCommand = CreateCommand(async () => await CreateExamAsync());
        DeleteExamCommand = CreateCommand<ExamDto>(async (exam) => await DeleteExamAsync(exam));
        LoadSubjectsCommand = CreateCommand(async () => await LoadSubjectsAsync()); // ДОДАЄМО

        Title = "Exams";
    }

    public override async Task InitializeAsync(object? parameter = null)
    {
        await LoadSubjectsAsync();
        await LoadExamsAsync();
        await LoadUpcomingExamsAsync();
    }

    private async Task LoadSubjectsAsync()
    {
        try
        {
            var subjects = await _subjectService.GetAllAsync();
            Subjects.Clear();
            foreach (var subject in subjects)
            {
                Subjects.Add(subject);
            }

            if (Subjects.Count > 0)
            {
                SelectedSubjectId = Subjects[0].Id;
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка завантаження предметів: {ex.Message}");
        }
    }

    private async Task LoadExamsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var query = new GetUserExamsQuery(_userContext.CurrentUserId);
            var exams = await _mediator.Send(query);

            AllExams.Clear();
            foreach (var exam in exams)
            {
                AllExams.Add(exam);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка завантаження екзаменів: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadUpcomingExamsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var query = new GetUpcomingExamsQuery(_userContext.CurrentUserId, 30);
            var upcomingExams = await _mediator.Send(query);

            UpcomingExams.Clear();
            foreach (var exam in upcomingExams)
            {
                UpcomingExams.Add(exam);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка завантаження майбутніх екзаменів: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CreateExamAsync()
    {
        if (string.IsNullOrWhiteSpace(NewExamTitle))
        {
            await _dialogService.ShowAlertAsync("Будь ласка, введіть назву екзамену");
            return;
        }

        if (SelectedSubjectId == 0)
        {
            await _dialogService.ShowAlertAsync("Будь ласка, виберіть предмет");
            return;
        }

        try
        {
            var command = new CreateExamCommand(
                _userContext.CurrentUserId,
                SelectedSubjectId,
                NewExamTitle,
                NewExamDate,
                NewExamStartTime,
                NewExamEndTime,
                NewExamDescription
            );

            await _mediator.Send(command);

            // Очистити поля
            NewExamTitle = string.Empty;
            NewExamDate = DateTime.Today.AddDays(7);
            NewExamStartTime = new TimeSpan(9, 0, 0);
            NewExamEndTime = new TimeSpan(11, 0, 0);
            NewExamDescription = string.Empty;

            // Перезавантажити
            await LoadExamsAsync();
            await LoadUpcomingExamsAsync();

            await _dialogService.ShowAlertAsync("Екзамен успішно створено");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка створення екзамену: {ex.Message}");
        }
    }

    private async Task DeleteExamAsync(ExamDto exam)
    {
        if (exam == null) return;

        var confirm = await _dialogService.ShowConfirmationAsync($"Видалити екзамен '{exam.Title}'?");
        if (!confirm) return;

        try
        {
            var command = new DeleteExamCommand(exam.Id);
            await _mediator.Send(command);

            await LoadExamsAsync();
            await LoadUpcomingExamsAsync();

            await _dialogService.ShowAlertAsync("Екзамен успішно видалено");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка видалення екзамену: {ex.Message}");
        }
    }
}