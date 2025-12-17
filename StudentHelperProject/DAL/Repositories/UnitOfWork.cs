using DAL.Data;
using DAL.Interfaces;
using DAL.Repositories;
using System;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        
        private IUserRepository _users;
        private ITaskRepository _tasks;
        private IEventRepository _events;
        private IExamRepository _exams;
        private ISubjectRepository _subjects;
        private INoteRepository _notes;
        private IGroupAcademicRepository _groupAcademics;
        private INotificationRepository _notifications;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public ITaskRepository Tasks => _tasks ??= new TaskRepository(_context);
        public IEventRepository Events => _events ??= new EventRepository(_context);
        public IExamRepository Exams => _exams ??= new ExamRepository(_context);
        public ISubjectRepository Subjects => _subjects ??= new SubjectRepository(_context);
        public INoteRepository Notes => _notes ??= new NoteRepository(_context);
        public IGroupAcademicRepository GroupAcademics => _groupAcademics ??= new GroupAcademicRepository(_context);
        public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);

        // Прості репозиторії без спеціальних методів
        public IRepository<Lecturer> Lecturers => new BaseRepository<Lecturer>(_context);
        public IRepository<Room> Rooms => new BaseRepository<Room>(_context);
        public IRepository<ScheduleSource> ScheduleSources => new BaseRepository<ScheduleSource>(_context);
        public IRepository<AuthIdentity> AuthIdentities => new BaseRepository<AuthIdentity>(_context);
        public IRepository<Profile> Profiles => new BaseRepository<Profile>(_context);
        public IRepository<PasswordResetToken> PasswordResetTokens => new BaseRepository<PasswordResetToken>(_context);
        public IRepository<Integration> Integrations => new BaseRepository<Integration>(_context);
        public IRepository<SecurityLog> SecurityLogs => new BaseRepository<SecurityLog>(_context);
        public IRepository<AppLog> AppLogs => new BaseRepository<AppLog>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}