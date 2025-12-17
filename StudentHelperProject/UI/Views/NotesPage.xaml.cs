using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using StudentHelper.WPF.UI.ViewModels;
using StudentHelper.WPF.UI.ViewModels.Items;

namespace StudentHelper.WPF.UI.Views
{
    public partial class NotesPage : Page
    {
        private NotesViewModel _viewModel;

        public NotesPage()
        {
            InitializeComponent();

            _viewModel = ServiceLocator.GetService<NotesViewModel>();
            DataContext = _viewModel;

            _viewModel.Notes.CollectionChanged += (s, e) => RefreshNotes();

            RefreshNotes();
        }

        public void RefreshData()
        {
            if (_viewModel != null)
            {
                _viewModel.LoadNotesCommand.Execute(null);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.SearchText = SearchBox.Text;
        }

        private void RefreshNotes()
        {
            if (NotesContainer == null) return;

            NotesContainer.Children.Clear();

            foreach (var note in _viewModel.Notes)
            {
                NotesContainer.Children.Add(CreateNoteCard(note));
            }
        }

        private Border CreateNoteCard(NoteItemViewModel note)
        {
            var card = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 10),
                Background = note.IsPinned ? new SolidColorBrush(Color.FromRgb(255, 250, 205)) : Brushes.White,
                Tag = note
            };

            var panel = new StackPanel();

            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            if (note.IsPinned)
            {
                headerPanel.Children.Add(new TextBlock
                {
                    Text = "📌 ",
                    FontSize = 16,
                    VerticalAlignment = VerticalAlignment.Center
                });
            }

            headerPanel.Children.Add(new TextBlock
            {
                Text = note.Title,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap
            });

            panel.Children.Add(headerPanel);

            var contentText = note.Content.Length > 200
                ? note.Content.Substring(0, 200) + "..."
                : note.Content;

            panel.Children.Add(new TextBlock
            {
                Text = contentText,
                Margin = new Thickness(0, 8, 0, 0),
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.DarkGray
            });

            panel.Children.Add(new TextBlock
            {
                Text = $"Створено: {note.Created:dd.MM.yyyy HH:mm}",
                Margin = new Thickness(0, 8, 0, 0),
                FontSize = 11,
                Foreground = Brushes.Gray
            });

            var actionsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 8, 0, 0)
            };

            var pinBtn = new Button
            {
                Content = note.IsPinned ? "Відкріпити" : "Закріпити",
                Margin = new Thickness(0, 0, 8, 0),
                Padding = new Thickness(8, 4, 8, 4),
                Tag = note
            };
            pinBtn.Click += (s, e) =>
            {
                if (_viewModel.TogglePinCommand.CanExecute(note))
                {
                    _viewModel.TogglePinCommand.Execute(note);
                }
            };

            var deleteBtn = new Button
            {
                Content = "Видалити",
                Padding = new Thickness(8, 4, 8, 4),
                Background = new SolidColorBrush(Color.FromRgb(255, 200, 200)),
                Tag = note
            };
            deleteBtn.Click += (s, e) =>
            {
                if (_viewModel.DeleteNoteCommand.CanExecute(note))
                {
                    _viewModel.DeleteNoteCommand.Execute(note);
                }
            };

            actionsPanel.Children.Add(pinBtn);
            actionsPanel.Children.Add(deleteBtn);
            panel.Children.Add(actionsPanel);

            card.Child = panel;
            return card;
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNoteCommand.Execute(null);
        }
    }
}
