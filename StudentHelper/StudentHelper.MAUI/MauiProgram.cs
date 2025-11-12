using MediatR;
using Microsoft.Extensions.Logging;
using StudentHelper.MAUI.Services;
using StudentHelper.MAUI.ViewModels.Authentication;
using StudentHelper.MAUI.ViewModels.Main;
using StudentHelper.BLL.CQRS.Notes;
using System.Reflection;

namespace StudentHelper.MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// РЕЄСТРАЦІЯ СЕРВІСІВ MAUI
		builder.Services.AddSingleton<IAppService, AppService>();
		builder.Services.AddSingleton<IDialogService, DialogService>();
		builder.Services.AddSingleton<INavigationService, NavigationService>();
		builder.Services.AddSingleton<IUserContext, UserContext>();

		// РЕЄСТРАЦІЯ MediatR
		builder.Services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(typeof(CreateNoteCommand).Assembly);
			cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
		});

		// РЕЄСТРАЦІЯ ViewModels
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<ForgotPasswordViewModel>();
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<CalendarViewModel>();
		builder.Services.AddTransient<NotesViewModel>();
		builder.Services.AddTransient<TasksViewModel>();
		builder.Services.AddTransient<ExamsViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		builder.Services.AddScoped<StudentHelper.BLL.Services.IUserService, StudentHelper.BLL.Services.UserService>();
		builder.Services.AddScoped<StudentHelper.BLL.Abstractions.INoteService, StudentHelper.BLL.Services.NoteService>();
		builder.Services.AddScoped<StudentHelper.BLL.Abstractions.ITaskService, StudentHelper.BLL.Services.TaskService>();
		builder.Services.AddScoped<StudentHelper.BLL.Abstractions.IExamService, StudentHelper.BLL.Services.ExamService>();
		builder.Services.AddScoped<StudentHelper.BLL.Abstractions.ISubjectService, StudentHelper.BLL.Services.SubjectService>();
		builder.Services.AddScoped<StudentHelper.BLL.Abstractions.IGroupAcademicService, StudentHelper.BLL.Services.GroupAcademicService>();
		builder.Services.AddScoped<StudentHelper.BLL.Abstractions.INotificationSettingService, StudentHelper.BLL.Services.NotificationSettingService>();

		// РЕЄСТРАЦІЯ СТОРІНОК
		builder.Services.AddTransient<MainPage>();

		return builder.Build();
	}
}