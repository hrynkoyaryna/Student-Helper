using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Main
{
    public class ExamsViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public ICommand LoadExamsCommand { get; }
        public ICommand AddExamCommand { get; }

        public ExamsViewModel(
            IAppService appService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _appService = appService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            LoadExamsCommand = CreateCommand(async () => await LoadExamsAsync());
            AddExamCommand = CreateCommand(async () => await AddExamAsync());

            Title = "Exams";
        }

        public override async Task InitializeAsync(object? parameter = null)
        {
            await LoadExamsAsync();
        }

        private async Task LoadExamsAsync()
        {
            try
            {
                var exams = await _appService.GetUserExamsAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка завантаження екзаменів: {ex.Message}", "Помилка");
            }
        }

        private async Task AddExamAsync()
        {
            await _dialogService.ShowAlertAsync("Функція додавання екзамену буде реалізована найближчим часом", "Інформація");
        }
    }
}