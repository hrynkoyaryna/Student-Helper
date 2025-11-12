using System.Collections.ObjectModel;
using System.Windows.Input;
using MediatR;
using StudentHelper.BLL.CQRS.Notes;
using StudentHelper.BLL.DTOs;
using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Base;

namespace StudentHelper.MAUI.ViewModels.Main;

public class NotesViewModel : BaseViewModel
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;
    private readonly IDialogService _dialogService;

    public ObservableCollection<NoteDto> Notes { get; } = new();
    public ObservableCollection<NoteDto> PinnedNotes { get; } = new();

    public ICommand LoadNotesCommand { get; }
    public ICommand LoadPinnedNotesCommand { get; }
    public ICommand CreateNoteCommand { get; }
    public ICommand DeleteNoteCommand { get; }
    public ICommand TogglePinCommand { get; }

    private string _newNoteTitle = string.Empty;
    public string NewNoteTitle
    {
        get => _newNoteTitle;
        set => SetProperty(ref _newNoteTitle, value);
    }

    private string _newNoteContent = string.Empty;
    public string NewNoteContent
    {
        get => _newNoteContent;
        set => SetProperty(ref _newNoteContent, value);
    }

    private bool _newNoteIsPinned;
    public bool NewNoteIsPinned
    {
        get => _newNoteIsPinned;
        set => SetProperty(ref _newNoteIsPinned, value);
    }

    public NotesViewModel(IMediator mediator, IUserContext userContext, IDialogService dialogService)
    {
        _mediator = mediator;
        _userContext = userContext;
        _dialogService = dialogService;

        LoadNotesCommand = CreateCommand(async () => await LoadNotesAsync());
        LoadPinnedNotesCommand = CreateCommand(async () => await LoadPinnedNotesAsync());
        CreateNoteCommand = CreateCommand(async () => await CreateNoteAsync());
        DeleteNoteCommand = CreateCommand<NoteDto>(async (note) => await DeleteNoteAsync(note));
        TogglePinCommand = CreateCommand<NoteDto>(async (note) => await TogglePinAsync(note));

        Title = "Notes";
    }

    public override async Task InitializeAsync(object? parameter = null)
    {
        await LoadNotesAsync();
        await LoadPinnedNotesAsync();
    }

    private async Task LoadNotesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var query = new GetUserNotesQuery(_userContext.CurrentUserId);
            var notes = await _mediator.Send(query);

            Notes.Clear();
            foreach (var note in notes)
            {
                Notes.Add(note);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка завантаження нотаток: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadPinnedNotesAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var query = new GetPinnedNotesQuery(_userContext.CurrentUserId);
            var pinnedNotes = await _mediator.Send(query);

            PinnedNotes.Clear();
            foreach (var note in pinnedNotes)
            {
                PinnedNotes.Add(note);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка завантаження закріплених нотаток: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CreateNoteAsync()
    {
        if (string.IsNullOrWhiteSpace(NewNoteTitle))
        {
            await _dialogService.ShowAlertAsync("Будь ласка, введіть заголовок нотатки");
            return;
        }

        try
        {
            var command = new CreateNoteCommand(
                _userContext.CurrentUserId,
                NewNoteTitle,
                NewNoteContent,
                NewNoteIsPinned
            );

            await _mediator.Send(command);

            // Очистити поля
            NewNoteTitle = string.Empty;
            NewNoteContent = string.Empty;
            NewNoteIsPinned = false;

            // Перезавантажити
            await LoadNotesAsync();
            await LoadPinnedNotesAsync();

            await _dialogService.ShowAlertAsync("Нотатку успішно створено");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка створення нотатки: {ex.Message}");
        }
    }

    private async Task DeleteNoteAsync(NoteDto note)
    {
        if (note == null) return;

        var confirm = await _dialogService.ShowConfirmationAsync($"Видалити нотатку '{note.Title}'?");
        if (!confirm) return;

        try
        {
            var command = new DeleteNoteCommand(note.Id);
            await _mediator.Send(command);

            await LoadNotesAsync();
            await LoadPinnedNotesAsync();

            await _dialogService.ShowAlertAsync("Нотатку успішно видалено");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка видалення нотатки: {ex.Message}");
        }
    }

    private async Task TogglePinAsync(NoteDto note)
    {
        if (note == null) return;

        try
        {
            var command = new UpdateNoteCommand(
                note.Id,
                note.UserId,
                note.Title,
                note.Content,
                !note.IsPinned
            );

            await _mediator.Send(command);

            await LoadNotesAsync();
            await LoadPinnedNotesAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync($"Помилка оновлення нотатки: {ex.Message}");
        }
    }
}