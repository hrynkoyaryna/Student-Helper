using System;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Основні репозиторії
        IUserRepository Users { get; }
        ITaskRepository Tasks { get; }
        IEventRepository Events { get; }
        IExamRepository Exams { get; }
        ISubjectRepository Subjects { get; }
        INoteRepository Notes { get; }
        IGroupAcademicRepository GroupAcademics { get; }
        
        // Додаткові репозиторії
        INotificationRepository Notifications { get; }
        IRepository<Lecturer> Lecturers { get; }
        IRepository<Room> Rooms { get; }
        IRepository<ScheduleSource> ScheduleSources { get; }
        IRepository<AuthIdentity> AuthIdentities { get; }
        IRepository<Profile> Profiles { get; }
        IRepository<PasswordResetToken> PasswordResetTokens { get; }
        IRepository<Integration> Integrations { get; }
        IRepository<SecurityLog> SecurityLogs { get; }
        IRepository<AppLog> AppLogs { get; }
        
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}