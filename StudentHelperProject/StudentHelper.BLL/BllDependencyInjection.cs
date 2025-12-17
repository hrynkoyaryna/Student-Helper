// StudentHelper.BLL/BllDependencyInjection.cs
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.Configuration;
using StudentHelper.BLL.Services;

namespace StudentHelper.BLL
{
    public static class BllDependencyInjection
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // Configure Email Settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Task Service
            services.AddScoped<ITaskService, TaskService>();
            // Exam Service
            services.AddScoped<IExamService, ExamService>();
            // Event Service
            services.AddScoped<IEventService, EventService>();
            // Subject Service
            services.AddScoped<ISubjectService, SubjectService>();
            // Note Service
            services.AddScoped<INoteService, NoteService>();
            // Notification Setting Service
            services.AddScoped<INotificationSettingService, NotificationSettingService>();
            // Group Academic Service
            services.AddScoped<IGroupAcademicService, GroupAcademicService>();
            // User Service
            services.AddScoped<IUserService, UserService>();
            // Email Service
            services.AddScoped<IEmailService, EmailService>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}