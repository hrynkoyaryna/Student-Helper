using System.Windows;

namespace StudentHelper.WPF.UI.Views.Dialogs
{
    public partial class InputDialog : Window
    {
        public string Answer => InputBox.Text.Trim();

        public InputDialog(string title, string message)
        {
            InitializeComponent();
            Title = title;
            MessageText.Text = message;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputBox.Text))
            {
                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}