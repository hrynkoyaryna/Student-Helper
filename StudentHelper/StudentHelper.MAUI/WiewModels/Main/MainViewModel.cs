using System.Windows.Input;
using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;

namespace StudentHelper.MAUI.ViewModels.Main;

public class MainViewModel : BaseViewModel
{
    private readonly IUserContext _userContext;

    public ICommand NavigateToNotesCommand { get; }
    public ICommand NavigateToTasksCommand { get; }
    public ICommand NavigateToExamsCommand { get; }
    public ICommand NavigateToCalendarCommand { get; }
    public ICommand NavigateToSettingsCommand { get; }

    public MainViewModel(IUserContext userContext)
    {
        _userContext = userContext;
        Title = "Student Helper";

        NavigateToNotesCommand = CreateCommand(async () => await NavigateToNotesAsync());
        NavigateToTasksCommand = CreateCommand(async () => await NavigateToTasksAsync());
        NavigateToExamsCommand = CreateCommand(async () => await NavigateToExamsAsync());
        NavigateToCalendarCommand = CreateCommand(async () => await NavigateToCalendarAsync());
        NavigateToSettingsCommand = CreateCommand(async () => await NavigateToSettingsAsync());
    }

    private async Task NavigateToNotesAsync()
    {
        await Shell.Current.GoToAsync("//notes");
    }

    private async Task NavigateToTasksAsync()
    {
        await Shell.Current.GoToAsync("//tasks");
    }

    private async Task NavigateToExamsAsync()
    {
        await Shell.Current.GoToAsync("//exams");
    }

    private async Task NavigateToCalendarAsync()
    {
        await Shell.Current.GoToAsync("//calendar");
    }

    private async Task NavigateToSettingsAsync()
    {
        await Shell.Current.GoToAsync("//settings");
    }
}