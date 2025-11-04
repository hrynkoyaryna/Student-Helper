using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Authentication;
using StudentHelper.MAUI.ViewModels.Main;

namespace StudentHelper.MAUI
{
	public static class MauiProgram  // ← Назва файлу має бути MauiProgram.cs
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()  // ← Виправлено: App, не MainApp
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

#if DEBUG
			builder.Logging.AddDebug();
#endif

			// Реєстрація сервісів
			builder.Services.AddSingleton<IAppService, AppService>();
			builder.Services.AddSingleton<IDialogService, DialogService>();
			builder.Services.AddSingleton<INavigationService, NavigationService>();

			// Реєстрація ViewModels
			builder.Services.AddTransient<LoginViewModel>();
			builder.Services.AddTransient<RegisterViewModel>();
			builder.Services.AddTransient<ForgotPasswordViewModel>();
			builder.Services.AddTransient<MainViewModel>();
			builder.Services.AddTransient<CalendarViewModel>();
			builder.Services.AddTransient<TasksViewModel>();
			builder.Services.AddTransient<ExamsViewModel>();
			builder.Services.AddTransient<NotesViewModel>();
			builder.Services.AddTransient<SettingsViewModel>();

			return builder.Build();
		}
	}
}