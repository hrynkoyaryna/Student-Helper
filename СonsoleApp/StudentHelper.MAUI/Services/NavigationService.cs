using StudentHelper.MAUI.ViewModels.Base;

namespace StudentHelper.MAUI.Services
{
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
            var viewModel = _serviceProvider.GetService(viewModelType) as BaseViewModel;
            if (viewModel != null)
            {
                await viewModel.InitializeAsync(parameter);


            }
        }

        public async Task GoBackAsync()
        {
            if (Shell.Current != null)
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}