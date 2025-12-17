using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StudentHelper.WPF.UI.ViewModels;
using StudentHelper.WPF.UI.ViewModels.Items;

namespace StudentHelper.WPF.UI.Views
{
    public partial class TasksPage : Page
    {
        private TasksViewModel _viewModel;

        public TasksPage()
        {
            InitializeComponent();

            _viewModel = ServiceLocator.GetService<TasksViewModel>();
            DataContext = _viewModel;

            _viewModel.Tasks.CollectionChanged += (s, e) => RefreshTaskList();

            RefreshTaskList();
        }

        public void RefreshData()
        {
            if (_viewModel != null)
            {
                _viewModel.LoadTasksCommand.Execute(null);
            }
        }

        private void RefreshTaskList()
        {
            if (TaskList == null) return;

            TaskList.Items.Clear();

            foreach (var task in _viewModel.Tasks)
            {
                var card = CreateTaskCard(task);
                TaskList.Items.Add(card);
            }
        }

        private Border CreateTaskCard(TaskItemViewModel task)
        {
            var card = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 8),
                Background = task.IsCompleted ? new SolidColorBrush(Color.FromRgb(230, 230, 230)) : Brushes.White
            };

            var panel = new StackPanel();

            // Title with checkbox
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };

            var checkBox = new CheckBox
            {
                IsChecked = task.IsCompleted,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0)
            };
            checkBox.Checked += (s, e) => ToggleTask(task);
            checkBox.Unchecked += (s, e) => ToggleTask(task);

            var titleText = new TextBlock
            {
                Text = task.Title,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : null
            };

            headerPanel.Children.Add(checkBox);
            headerPanel.Children.Add(titleText);
            panel.Children.Add(headerPanel);

            // Description
            if (!string.IsNullOrWhiteSpace(task.Description))
            {
                panel.Children.Add(new TextBlock
                {
                    Text = task.Description,
                    Margin = new Thickness(24, 4, 0, 0),
                    Foreground = Brushes.Gray,
                    TextWrapping = TextWrapping.Wrap
                });
            }

            // Due date
            panel.Children.Add(new TextBlock
            {
                Text = $"Дата: {task.DueDate:dd.MM.yyyy}",
                Margin = new Thickness(24, 4, 0, 0),
                FontSize = 11,
                Foreground = task.DueDate < DateTime.UtcNow ? Brushes.Red : Brushes.Gray
            });

            // Delete button
            var deleteBtn = new Button
            {
                Content = "Видалити",
                Margin = new Thickness(24, 8, 0, 0),
                Padding = new Thickness(8, 4, 8, 4),
                Background = new SolidColorBrush(Color.FromRgb(255, 200, 200)),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            deleteBtn.Click += (s, e) => DeleteTask(task);

            panel.Children.Add(deleteBtn);
            card.Child = panel;
            return card;
        }

        private void ToggleTask(TaskItemViewModel task)
        {
            if (_viewModel.ToggleTaskCommand.CanExecute(task))
            {
                _viewModel.ToggleTaskCommand.Execute(task);
            }
        }

        private void DeleteTask(TaskItemViewModel task)
        {
            if (_viewModel.DeleteTaskCommand.CanExecute(task))
            {
                _viewModel.DeleteTaskCommand.Execute(task);
            }
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null || Tabs == null) return;

            _viewModel.SelectedTab = Tabs.SelectedIndex switch
            {
                0 => "current",
                1 => "done",
                2 => "overdue",
                _ => "current"
            };
        }

        private void TaskTypeChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            if (StudyFilter?.IsChecked == true)
            {
                _viewModel.SelectedCategory = "Навчання";
            }
            else if (PersonalFilter?.IsChecked == true)
            {
                _viewModel.SelectedCategory = "Особисте";
            }
        }

        private void SubjectChanged(object sender, SelectionChangedEventArgs e)
        {
            // Subject filtering only makes sense for Study tasks
            if (_viewModel == null || SubjectFilter == null || SubjectFilter.SelectedItem == null) return;

            string selectedSubject = (SubjectFilter.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            // If "Без предмета" is selected, show all tasks
            if (selectedSubject == "Без предмета" || string.IsNullOrWhiteSpace(selectedSubject))
            {
                RefreshTaskList();
                return;
            }

            // Filter by subject (only relevant for Study tasks)
            if (TaskList == null) return;

            TaskList.Items.Clear();

            foreach (var task in _viewModel.Tasks)
            {
                // Only filter Study tasks (those with SubjectId)
                if (task.SubjectId.HasValue)
                {
                    // Check if subject matches
                    if (task.Subject == selectedSubject ||
                        task.Subject?.Contains(selectedSubject) == true)
                    {
                        var card = CreateTaskCard(task);
                        TaskList.Items.Add(card);
                    }
                }
                else if (_viewModel.SelectedCategory == "Особисте")
                {
                    // Show personal tasks regardless of subject filter
                    var card = CreateTaskCard(task);
                    TaskList.Items.Add(card);
                }
            }
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new Dialogs.InputDialog("Новий предмет", "Введіть новий предмет:");

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
                    // Add new subject to filter
                    var newItem = new ComboBoxItem { Content = newSubject };
                    SubjectFilter.Items.Add(newItem);
                    SubjectFilter.SelectedItem = newItem;

                    MessageBox.Show($"Предмет '{newSubject}' додано до списку", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Цей предмет уже існує в списку", "Повідомлення", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddTaskCommand.Execute(null);
        }
    }
}
