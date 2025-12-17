using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;
using StudentHelper.WPF.UI.ViewModels;
using StudentHelper.WPF.UI.ViewModels.Items;

namespace StudentHelper.WPF.UI.Views
{
    public partial class ExamsPage : Page
    {
        private ExamsViewModel _viewModel;
        private ISubjectService _subjectService;

        public ExamsPage()
        {
            InitializeComponent();

            _viewModel = ServiceLocator.GetService<ExamsViewModel>();
            _subjectService = ServiceLocator.GetService<ISubjectService>();
            DataContext = _viewModel;

            _viewModel.Exams.CollectionChanged += (s, e) => RefreshExamList();

            _ = LoadSubjectsToFilterAsync();
            RefreshExamList();
        }

        public void RefreshData()
        {
            if (_viewModel != null)
            {
                _viewModel.LoadExamsCommand.Execute(null);
            }
        }

        private async Task LoadSubjectsToFilterAsync()
        {
            try
            {
                var subjects = await _subjectService.GetAllAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Зберіганни "Усі предмети"
                    var allItemsOption = SubjectFilter.Items[0] as ComboBoxItem;
                    SubjectFilter.Items.Clear();
                    SubjectFilter.Items.Add(allItemsOption ?? new ComboBoxItem { Content = "Усі предмети" });

                    // F�ge echte Subjects aus DB hinzu
                    foreach (var subject in subjects.OrderBy(s => s.Name))
                    {
                        var item = new ComboBoxItem { Content = subject.Name, Tag = subject.Id };

                        // Правий клік контекстне меню для видалення
                        var contextMenu = new ContextMenu();
                        var deleteMenuItem = new MenuItem { Header = "Видалити предмет" };
                        deleteMenuItem.Click += (s, e) => DeleteSubject_Click(subject.Id, subject.Name);
                        contextMenu.Items.Add(deleteMenuItem);
                        item.ContextMenu = contextMenu;

                        SubjectFilter.Items.Add(item);
                    }

                    SubjectFilter.SelectedIndex = 0;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження іспитів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshExamList()
        {
            if (ExamList == null) return;

            ExamList.Items.Clear();

            // Уникальні предмети з іспитів для фільтра
            var subjects = new HashSet<string>();
            foreach (var exam in _viewModel.Exams)
            {
                if (!string.IsNullOrWhiteSpace(exam.Subject))
                {
                    subjects.Add(exam.Subject);
                }
            }

            // Відновити вибір користувача, зберігаючи "Усі предмети"
            if (SubjectFilter != null)
            {
                var currentSelection = SubjectFilter.SelectedItem as ComboBoxItem;
                var allItemsOption = SubjectFilter.Items[0] as ComboBoxItem; // "Усі предмети"

                SubjectFilter.Items.Clear();
                SubjectFilter.Items.Add(allItemsOption ?? new ComboBoxItem { Content = "Усі предмети" });

                foreach (var subject in subjects.OrderBy(s => s))
                {
                    SubjectFilter.Items.Add(new ComboBoxItem { Content = subject });
                }

                // Вибрати попередній вибір предмету "Усі предмети"
                if (currentSelection != null)
                {
                    string currentSubject = currentSelection.Content?.ToString() ?? "";
                    foreach (ComboBoxItem item in SubjectFilter.Items)
                    {
                        if (item.Content?.ToString() == currentSubject)
                        {
                            SubjectFilter.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (SubjectFilter.SelectedItem == null)
                {
                    SubjectFilter.SelectedIndex = 0; // "Усі предмети"
                }
            }

            // Перемістити іспити з фільтром
            foreach (var exam in _viewModel.Exams)
            {
                var card = CreateExamCard(exam);
                ExamList.Items.Add(card);
            }
        }

        private Border CreateExamCard(ExamItemViewModel exam)
        {
            var card = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 8),
                Background = exam.IsPassed ? new SolidColorBrush(Color.FromRgb(200, 255, 200)) : Brushes.White
            };

            var panel = new StackPanel();

            // Title
            var titlePanel = new StackPanel { Orientation = Orientation.Horizontal };

            if (exam.IsPassed)
            {
                titlePanel.Children.Add(new TextBlock
                {
                    Text = "✓ ",
                    FontSize = 16,
                    Foreground = Brushes.Green,
                    FontWeight = FontWeights.Bold
                });
            }

            titlePanel.Children.Add(new TextBlock
            {
                Text = exam.Title,
                FontSize = 14,
                FontWeight = FontWeights.Bold
            });

            panel.Children.Add(titlePanel);

            // Date
            var daysUntil = (exam.Date - DateTime.UtcNow).Days;
            var dateColor = daysUntil < 0 ? Brushes.Gray : (daysUntil <= 7 ? Brushes.Red : Brushes.DarkBlue);

            panel.Children.Add(new TextBlock
            {
                Text = $"Дата: {exam.Date:dd.MM.yyyy}",
                Margin = new Thickness(0, 4, 0, 0),
                FontSize = 13,
                Foreground = dateColor,
                FontWeight = daysUntil <= 7 && daysUntil >= 0 ? FontWeights.Bold : FontWeights.Normal
            });

            if (daysUntil >= 0 && daysUntil <= 30)
            {
                panel.Children.Add(new TextBlock
                {
                    Text = $"Залишилось днів: {daysUntil}",
                    Margin = new Thickness(0, 2, 0, 0),
                    FontSize = 11,
                    Foreground = dateColor
                });
            }

            // Description
            if (!string.IsNullOrWhiteSpace(exam.Description))
            {
                panel.Children.Add(new TextBlock
                {
                    Text = exam.Description,
                    Margin = new Thickness(0, 4, 0, 0),
                    Foreground = Brushes.Gray,
                    TextWrapping = TextWrapping.Wrap
                });
            }

            // Actions
            var actionsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 8, 0, 0)
            };

            var passedBtn = new Button
            {
                Content = exam.IsPassed ? "Позначити як невдалий" : "Позначити як вдалий",
                Margin = new Thickness(0, 0, 8, 0),
                Padding = new Thickness(8, 4, 8, 4)
            };
            passedBtn.Click += (s, e) => TogglePassed(exam);

            var editBtn = new Button
            {
                Content = "Редагувати",
                Margin = new Thickness(0, 0, 8, 0),
                Padding = new Thickness(8, 4, 8, 4),
                Background = new SolidColorBrush(Color.FromRgb(200, 230, 255))
            };
            editBtn.Click += (s, e) => EditExam(exam);

            var deleteBtn = new Button
            {
                Content = "Видалити",
                Padding = new Thickness(8, 4, 8, 4),
                Background = new SolidColorBrush(Color.FromRgb(255, 200, 200))
            };
            deleteBtn.Click += (s, e) => DeleteExam(exam);

            actionsPanel.Children.Add(passedBtn);
            actionsPanel.Children.Add(editBtn);
            actionsPanel.Children.Add(deleteBtn);
            panel.Children.Add(actionsPanel);

            card.Child = panel;
            return card;
        }

        private void TogglePassed(ExamItemViewModel exam)
        {
            if (_viewModel.TogglePassedCommand.CanExecute(exam))
            {
                _viewModel.TogglePassedCommand.Execute(exam);
            }
        }

        private void DeleteExam(ExamItemViewModel exam)
        {
            if (_viewModel.DeleteExamCommand.CanExecute(exam))
            {
                _viewModel.DeleteExamCommand.Execute(exam);
            }
        }

        private void EditExam(ExamItemViewModel exam)
        {
            if (_viewModel.EditExamCommand.CanExecute(exam))
            {
                _viewModel.EditExamCommand.Execute(exam);
            }
        }

        private void Tabs_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null || Tabs == null) return;

            _viewModel.SelectedTab = Tabs.SelectedIndex == 0 ? "current" : "past";
        }

        private void SubjectFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (SubjectFilter == null || SubjectFilter.SelectedItem == null) return;

            string selectedSubject = (SubjectFilter.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            // Якщо вибрано "Усі предмети", показувати все
            if (selectedSubject == "Усі предмети" || string.IsNullOrWhiteSpace(selectedSubject))
            {
                RefreshExamList();
                return;
            }

            // Фільтрувати за предметом
            if (ExamList == null) return;

            ExamList.Items.Clear();

            foreach (var exam in _viewModel.Exams)
            {
                // Фільтрувати за вибраним предметом
                if (exam.Subject == selectedSubject ||
                    exam.Subject?.Contains(selectedSubject) == true)
                {
                    var card = CreateExamCard(exam);
                    ExamList.Items.Add(card);
                }
            }
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new Dialogs.InputDialog("Новий предмет", "Введіть назву предмета:");

            if (inputDialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(inputDialog.Answer))
            {
                string newSubject = inputDialog.Answer.Trim();

                // Check if subject already exists
                bool exists = false;
                foreach (ComboBoxItem item in SubjectFilter.Items)
                {
                    if (item.Content?.ToString() == newSubject)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    // Add new subject to filter (after "Усі предмети")
                    var newItem = new ComboBoxItem { Content = newSubject };
                    SubjectFilter.Items.Add(newItem);
                    SubjectFilter.SelectedItem = newItem;

                    MessageBox.Show($"Предмет '{newSubject}' додан до списку", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Цей предмет уже існує в списку", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void AddExam_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddExamCommand.Execute(null);
        }

        private async void DeleteSubject_Click(int subjectId, string subjectName)
        {
            var result = MessageBox.Show(
                $"Видалити предмет \"{subjectName}\"?\n\nУвага: Це може видалити пов'язані іспити!",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _subjectService.DeleteAsync(subjectId);

                    // Reload subjects
                    await LoadSubjectsToFilterAsync();

                    // Reload exams
                    _viewModel.LoadExamsCommand.Execute(null);

                    MessageBox.Show($"Предмет \"{subjectName}\" видалено", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка видалення предмета: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
