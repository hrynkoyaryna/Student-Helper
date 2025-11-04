using Microsoft.Maui.Controls;
using StudentHelper.MAUI.Services;

namespace StudentHelper.MAUI
{
	public partial class App : Application
	{
		public App(INavigationService navigationService)
		{
			InitializeComponent();
			MainPage = new AppShell();
			_ = navigationService.InitializeAsync();
		}
	}
}