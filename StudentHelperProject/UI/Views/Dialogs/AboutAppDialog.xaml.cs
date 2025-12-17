using System.Windows;

namespace StudentHelper.WPF.UI.Views.Dialogs
{
    public partial class AboutAppDialog : Window
    {
        public AboutAppDialog()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
