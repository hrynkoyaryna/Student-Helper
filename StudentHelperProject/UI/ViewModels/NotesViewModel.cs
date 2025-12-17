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
    public class NotesViewModel : ViewModelBase
    {
        private readonly INoteService _noteService;
        private bool _showOnlyPinned;
        private bool _isLoading;
        private string _searchText = string.Empty;

        public NotesViewModel(INoteService noteService)
        {
            _noteService = noteService;

            Notes = new ObservableCollection<NoteItemViewModel>();

            LoadNotesCommand = new RelayCommand(async _ => await LoadNotesAsync());
            TogglePinCommand = new RelayCommand<NoteItemViewModel>(async n => await TogglePinAsync(n));
            DeleteNoteCommand = new RelayCommand<NoteItemViewModel>(async n => await DeleteNoteAsync(n));
            AddNoteCommand = new RelayCommand(_ => AddNote());
        }

        public ObservableCollection<NoteItemViewModel> Notes { get; }

        public bool ShowOnlyPinned
        {
            get => _showOnlyPinned;
            set
            {
                if (SetProperty(ref _showOnlyPinned, value))
                {
                    _ = LoadNotesAsync();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _ = LoadNotesAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadNotesCommand { get; }
        public ICommand TogglePinCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand AddNoteCommand { get; }

        private async Task LoadNotesAsync()
        {
            if (!UserSession.IsAuthenticated) return;

            try
            {
                IsLoading = true;

                var notes = ShowOnlyPinned
                    ? await _noteService.GetPinnedNotesAsync(UserSession.CurrentUserId)
                    : await _noteService.GetUserNotesAsync(UserSession.CurrentUserId);

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    notes = notes.Where(n =>
                        n.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                        n.Content.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notes.Clear();
                    foreach (var note in notes.OrderByDescending(n => n.IsPinned).ThenByDescending(n => n.UpdatedAt))
                    {
                        Notes.Add(MapToViewModel(note));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження записів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task TogglePinAsync(NoteItemViewModel? noteViewModel)
        {
            if (noteViewModel == null) return;

            try
            {
                var dto = new NoteDto(
                    noteViewModel.NoteId,
                    UserSession.CurrentUserId,
                    noteViewModel.Title,
                    noteViewModel.Content,
                    !noteViewModel.IsPinned,
                    noteViewModel.Created,
                    DateTime.UtcNow
                );

                await _noteService.UpdateAsync(dto);
                await LoadNotesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteNoteAsync(NoteItemViewModel? noteViewModel)
        {
            if (noteViewModel == null) return;

            var result = MessageBox.Show(
                $"Видалити запис \"{noteViewModel.Title}\"?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _noteService.DeleteAsync(noteViewModel.NoteId);
                    await LoadNotesAsync();
                    MessageBox.Show("Запис видалено", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddNote()
        {
            var dialog = new Views.Dialogs.CreateNoteDialog
            {
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true && dialog.NewNote != null)
            {
                var dto = new NoteDto(
                    0,
                    UserSession.CurrentUserId,
                    string.IsNullOrWhiteSpace(dialog.NewNote.Title) ? "Без назви" : dialog.NewNote.Title,
                    string.IsNullOrWhiteSpace(dialog.NewNote.Content) ? string.Empty : dialog.NewNote.Content,
                    false,
                    DateTime.UtcNow,
                    DateTime.UtcNow
                );

                Task.Run(async () =>
                {
                    try
                    {
                        await _noteService.CreateAsync(dto);
                        await LoadNotesAsync();

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show("Нотатка додана", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Помилка при додаванні: {ex.Message}\n\nInner: {ex.InnerException?.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                });
            }
        }

        private NoteItemViewModel MapToViewModel(NoteDto dto)
        {
            return new NoteItemViewModel
            {
                NoteId = dto.Id,
                Title = dto.Title,
                Content = dto.Content,
                Created = dto.CreatedAt,
                IsPinned = dto.IsPinned
            };
        }
    }
}

