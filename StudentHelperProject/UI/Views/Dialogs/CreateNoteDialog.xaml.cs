using System;
using System.Windows;
using StudentHelper.WPF.UI.ViewModels.Items;

namespace StudentHelper.WPF.UI.Views.Dialogs
{
    public partial class CreateNoteDialog : Window
    {
        public NoteItemViewModel? NewNote { get; private set; }
        private readonly NoteItemViewModel? _existingNote;

        // ---------- MODE: CREATE ----------
        public CreateNoteDialog()
        {
            InitializeComponent();
        }

        // ---------- MODE: EDIT ----------
        public CreateNoteDialog(NoteItemViewModel note)
        {
            InitializeComponent();
            _existingNote = note;

            DialogTitle.Text = "Редагування запису";
            Title = "Редагування запису";

            TitleBox.Text = note.Title;
            ContentBox.Text = note.Content;
            PinCheckbox.IsChecked = note.IsPinned;
        }

        // ---------- SAVE ----------
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                MessageBox.Show("Заголовок не може бути пустим.", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_existingNote != null)
            {
                // Оновити існуючий запис
                _existingNote.Title = TitleBox.Text.Trim();
                _existingNote.Content = ContentBox.Text.Trim();
                _existingNote.IsPinned = PinCheckbox.IsChecked == true;

                DialogResult = true;
                Close();
                return;
            }

            // Створити новий запис
            NewNote = new NoteItemViewModel
            {
                Title = TitleBox.Text.Trim(),
                Content = ContentBox.Text.Trim(),
                Created = DateTime.Now,
                IsPinned = PinCheckbox.IsChecked == true
            };

            DialogResult = true;
            Close();
        }

        // ---------- CANCEL ----------
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
