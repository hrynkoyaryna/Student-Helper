using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;
using StudentHelper.WPF.UI.ViewModels.Items;
using StudentHelper.WPF.UI.Services;

namespace StudentHelper.WPF.UI.ViewModels
{
    public class ExamsViewModel : ViewModelBase
    {
        private readonly IExamService _examService;
        private readonly ISubjectService _subjectService;
        private readonly ILogger<ExamsViewModel>? _logger;
        private string _selectedTab = "current";
        private bool _isLoading;
        private Dictionary<int, string> _subjectNames = new();
        private static readonly MemoryCache<string, List<SubjectDto>> _subjectCache =
            new(TimeSpan.FromMinutes(10));

        public ExamsViewModel(IExamService examService, ISubjectService subjectService, ILogger<ExamsViewModel>? logger = null)
        {
            _examService = examService;
            _subjectService = subjectService;
            _logger = logger;

            _logger?.LogInformation("Initializing ExamsViewModel");

            Exams = new ObservableCollection<ExamItemViewModel>();

            LoadExamsCommand = new RelayCommand(async _ => await LoadExamsAsync());
            TogglePassedCommand = new RelayCommand<ExamItemViewModel>(async e => await TogglePassedAsync(e));
            DeleteExamCommand = new RelayCommand<ExamItemViewModel>(async e => await DeleteExamAsync(e));
            AddExamCommand = new RelayCommand(_ => AddExam());
            EditExamCommand = new RelayCommand<ExamItemViewModel>(e => EditExam(e));
        }

        /// <summary>
        /// Asynchronously initializes the ViewModel by loading subjects and exams
        /// </summary>
        private async Task InitializeAsync()
        {
            try
            {
                _logger?.LogInformation("Starting ExamsViewModel initialization");
                await LoadSubjectsAsync();
                await LoadExamsAsync();
                _logger?.LogInformation("ExamsViewModel initialized successfully");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error initializing ExamsViewModel");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Помилка ініціалізації: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        public ObservableCollection<ExamItemViewModel> Exams { get; }

        public string SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (SetProperty(ref _selectedTab, value))
                {
                    _ = LoadExamsAsync();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadExamsCommand { get; }
        public ICommand TogglePassedCommand { get; }
        public ICommand DeleteExamCommand { get; }
        public ICommand AddExamCommand { get; }
        public ICommand EditExamCommand { get; }

        private async Task LoadSubjectsAsync()
        {
            try
            {
                _logger?.LogInformation("Loading subjects...");

                // Try cache first
                if (_subjectCache.TryGet("all", out var cachedSubjects))
                {
                    _logger?.LogInformation("Subjects loaded from cache");
                    _subjectNames = cachedSubjects!.ToDictionary(s => s.Id, s => s.Name);
                    return;
                }

                // Load from database
                var subjects = await _subjectService.GetAllAsync();
                _subjectNames = subjects.ToDictionary(s => s.Id, s => s.Name);

                // Update cache
                _subjectCache.Set("all", subjects);

                _logger?.LogInformation("Loaded {Count} subjects from database", subjects.Count);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading subjects");
                MessageBox.Show($"Помилка завантаження предметів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadExamsAsync()
        {
            if (!UserSession.IsAuthenticated) return;

            try
            {
                IsLoading = true;

                var exams = await _examService.GetUserExamsAsync(UserSession.CurrentUserId);

                var filtered = SelectedTab == "current"
                    ? exams.Where(e => e.ExamDate >= DateTime.Today).ToList()
                    : exams.Where(e => e.ExamDate < DateTime.Today).ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Exams.Clear();
                    foreach (var exam in filtered.OrderBy(e => e.ExamDate))
                    {
                        Exams.Add(MapToViewModel(exam));
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження іспитів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task TogglePassedAsync(ExamItemViewModel? examViewModel)
        {
            if (examViewModel == null || examViewModel.ExamId <= 0) return;

            try
            {
                // Toggle the status
                examViewModel.IsPassed = !examViewModel.IsPassed;

                // Update in database
                var dto = new ExamDto(
                    examViewModel.ExamId,
                    UserSession.CurrentUserId,
                    examViewModel.SubjectId ?? AppConstants.DefaultSubjectId,
                    examViewModel.Title,
                    examViewModel.Date,
                    TimeSpan.Zero,
                    TimeSpan.Zero,
                    examViewModel.Description ?? string.Empty
                );

                await _examService.UpdateAsync(dto);

                MessageBox.Show(
                    examViewModel.IsPassed ? "Іспит позначено як складений" : "Іспит позначено як незавершений",
                    "Успіх",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Reload to ensure consistency
                await LoadExamsAsync();
            }
            catch (Exception ex)
            {
                // Revert the change on error
                examViewModel.IsPassed = !examViewModel.IsPassed;
                MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteExamAsync(ExamItemViewModel? examViewModel)
        {
            if (examViewModel == null) return;

            var result = MessageBox.Show(
                $"Видалити іспит \"{examViewModel.Title}\"?",
                "Підтвердження",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _examService.DeleteAsync(examViewModel.ExamId);
                    await LoadExamsAsync();
                    MessageBox.Show("Іспит видалено", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddExam()
        {
            // Check if user is authenticated
            if (!UserSession.IsAuthenticated)
            {
                MessageBox.Show("Будь ласка, увійдіть в систему", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dialog = new Views.Dialogs.CreateExamDialog
                {
                    Owner = Application.Current.MainWindow
                };

                bool? dialogResult = dialog.ShowDialog();

                if (dialogResult == true && dialog.NewExam != null)
                {
                    // Verwende die tatsächliche SubjectId aus dem Dialog
                    var dto = new ExamDto(
                        0,
                        UserSession.CurrentUserId,
                        dialog.NewExam.SubjectId ?? AppConstants.DefaultSubjectId,
                        dialog.NewExam.Title,
                        dialog.NewExam.Date,
                        TimeSpan.Zero,
                        TimeSpan.Zero,
                        string.IsNullOrWhiteSpace(dialog.NewExam.Description) ? string.Empty : dialog.NewExam.Description
                    );

                    Task.Run(async () =>
                    {
                        try
                        {
                            await _examService.CreateAsync(dto);
                            await LoadExamsAsync();

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Екзамен додано", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відкриття діалогу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditExam(ExamItemViewModel? examViewModel)
        {
            if (examViewModel == null) return;

            if (!UserSession.IsAuthenticated)
            {
                MessageBox.Show("Будь ласка, увійдіть в систему", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Get subject names for dialog
                var subjectNames = _subjectNames.Values.ToList();

                var dialog = new Views.Dialogs.EditExamDialog(examViewModel, subjectNames)
                {
                    Owner = Application.Current.MainWindow
                };

                bool? dialogResult = dialog.ShowDialog();

                if (dialogResult == true && dialog.EditedExam != null)
                {
                    var dto = new ExamDto(
                        examViewModel.ExamId,
                        UserSession.CurrentUserId,
                        dialog.EditedExam.SubjectId ?? AppConstants.DefaultSubjectId,
                        dialog.EditedExam.Title,
                        dialog.EditedExam.Date,
                        TimeSpan.Zero,
                        TimeSpan.Zero,
                        string.IsNullOrWhiteSpace(dialog.EditedExam.Description) ? string.Empty : dialog.EditedExam.Description
                    );

                    Task.Run(async () =>
                    {
                        try
                        {
                            await _examService.UpdateAsync(dto);
                            await LoadExamsAsync();

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Іспит оновлено", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                            });
                        }
                        catch (Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відкриття діалогу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ExamItemViewModel MapToViewModel(ExamDto dto)
        {
            return new ExamItemViewModel
            {
                ExamId = dto.Id,
                Title = dto.Title,
                Description = dto.Description ?? string.Empty,
                Subject = _subjectNames.GetValueOrDefault(dto.SubjectId, $"Subject {dto.SubjectId}"),
                SubjectId = dto.SubjectId,
                Date = dto.ExamDate,
                IsPassed = dto.ExamDate < DateTime.Today
            };
        }
    }
}
