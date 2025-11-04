using StudentHelper.MAUI.ViewModels.Base;

namespace StudentHelper.MAUI.Services
{
    public interface INavigationService
    {
        Task InitializeAsync();
        Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel;
        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel;
        Task GoBackAsync();
    }
}