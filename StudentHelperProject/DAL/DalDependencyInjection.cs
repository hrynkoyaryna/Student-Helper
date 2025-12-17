// DAL/DalDependencyInjection.cs
using DAL.Data;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public static class DalDependencyInjection
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                if (connectionString.Contains("Host="))
                {
                    options.UseNpgsql(connectionString);
                }
                else
                {
                    options.UseSqlite(connectionString);
                }

                // Disable thread safety checks for development
                options.EnableThreadSafetyChecks(false);
            }, ServiceLifetime.Scoped);

            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
            services.AddScoped<IGroupAcademicRepository, GroupAcademicRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            return services;
        }
    }
}