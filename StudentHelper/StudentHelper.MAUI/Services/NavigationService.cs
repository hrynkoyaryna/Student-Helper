using StudentHelper.MAUI.ViewModels.Base;

namespace StudentHelper.MAUI.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task InitializeAsync()
    {
        return NavigateToAsync<ViewModels.Authentication.LoginViewModel>();
    }

    public async Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel
    {
        await NavigateToPageAsync(typeof(TViewModel));
    }

    public async Task NavigateToAsync<TViewModel>(object? parameter) where TViewModel : BaseViewModel
    {
        await NavigateToPageAsync(typeof(TViewModel), parameter);
    }

    private async Task NavigateToPageAsync(Type viewModelType, object? parameter = null)
    {
        try
        {
            var viewModel = _serviceProvider.GetService(viewModelType) as BaseViewModel;
            if (viewModel != null)
            {
                await viewModel.InitializeAsync(parameter);

                var route = GetRouteForViewModel(viewModelType);
                await Shell.Current.GoToAsync(route);
            }
        }
        catch (Exception ex)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка навігації", ex.Message, "OK");
            }
        }
    }

    private string GetRouteForViewModel(Type viewModelType)
    {
        return viewModelType.Name switch
        {
            nameof(ViewModels.Authentication.LoginViewModel) => "//login",
            nameof(ViewModels.Authentication.RegisterViewModel) => "//register",
            nameof(ViewModels.Authentication.ForgotPasswordViewModel) => "//forgotpassword",
            nameof(ViewModels.Main.MainViewModel) => "//main",
            nameof(ViewModels.Main.CalendarViewModel) => "//calendar",
            nameof(ViewModels.Main.NotesViewModel) => "//notes",
            nameof(ViewModels.Main.TasksViewModel) => "//tasks",
            nameof(ViewModels.Main.ExamsViewModel) => "//exams",
            nameof(ViewModels.Main.SettingsViewModel) => "//settings",
            _ => "//main"
        };
    }

    public async Task GoBackAsync()
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}