using Microsoft.Maui.Controls;
using StudentHelper.MAUI.Services;

namespace StudentHelper.MAUI
{
	public partial class App : Application
	{
		public App(INavigationService navigationService)
		{
			try
			{
				InitializeComponent();
				MainPage = new AppShell();

				Task.Run(async () => await navigationService.InitializeAsync());
			}
			catch (Exception ex)
			{
				MainPage = new ContentPage
				{
					Content = new Label { Text = "Помилка запуску додатка: " + ex.Message }
				};
			}
		}
	}
}