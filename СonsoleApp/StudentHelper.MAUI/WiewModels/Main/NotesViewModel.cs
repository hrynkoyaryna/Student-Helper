using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;
using System.Windows.Input;

namespace StudentHelper.MAUI.ViewModels.Main
{
    public class NotesViewModel : BaseViewModel
    {
        private readonly IAppService _appService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public ICommand LoadNotesCommand { get; }
        public ICommand AddNoteCommand { get; }

        public NotesViewModel(
            IAppService appService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _appService = appService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            LoadNotesCommand = CreateCommand(async () => await LoadNotesAsync());
            AddNoteCommand = CreateCommand(async () => await AddNoteAsync());

            Title = "Notes";
        }

        public override async Task InitializeAsync(object? parameter = null)
        {
            await LoadNotesAsync();
        }

        private async Task LoadNotesAsync()
        {
            try
            {
                var notes = await _appService.GetUserNotesAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAlertAsync($"Помилка завантаження нотаток: {ex.Message}", "Помилка");
            }
        }

        private async Task AddNoteAsync()
        {
            await _dialogService.ShowAlertAsync("Функція додавання нотатки буде реалізована найближчим часом", "Інформація");
        }
    }
}