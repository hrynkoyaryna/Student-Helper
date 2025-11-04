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
        
        // Спеціалізовані репозиторії
        private IUserRepository? _users;
        private ITaskRepository? _tasks;
        private IEventRepository? _events;
        private IExamRepository? _exams;
        private ISubjectRepository? _subjects;
        private INoteRepository? _notes;
        private IGroupAcademicRepository? _groupAcademics;
        private INotificationRepository? _notifications;

        // Приватні поля для BaseRepository (якщо потрібно)
        private IRepository<Lecturer>? _lecturers;
        private IRepository<Room>? _rooms;
        private IRepository<ScheduleSource>? _scheduleSources;
        private IRepository<AuthIdentity>? _authIdentities;
        private IRepository<Profile>? _profiles;
        private IRepository<PasswordResetToken>? _passwordResetTokens;
        private IRepository<Integration>? _integrations;
        private IRepository<SecurityLog>? _securityLogs;
        private IRepository<AppLog>? _appLogs;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // --- Реалізація спеціалізованих репозиторіїв ---
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public ITaskRepository Tasks => _tasks ??= new TaskRepository(_context);
        public IEventRepository Events => _events ??= new EventRepository(_context);
        public IExamRepository Exams => _exams ??= new ExamRepository(_context);
        public ISubjectRepository Subjects => _subjects ??= new SubjectRepository(_context);
        public INoteRepository Notes => _notes ??= new NoteRepository(_context);
        public IGroupAcademicRepository GroupAcademics => _groupAcademics ??= new GroupAcademicRepository(_context);
        public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);

        // --- Реалізація загальних репозиторіїв (IRepository<T>) ---
        // Використовуємо тут BaseRepository
        public IRepository<Lecturer> Lecturers => _lecturers ??= new BaseRepository<Lecturer>(_context);
        public IRepository<Room> Rooms => _rooms ??= new BaseRepository<Room>(_context);
        public IRepository<ScheduleSource> ScheduleSources => _scheduleSources ??= new BaseRepository<ScheduleSource>(_context);
        public IRepository<AuthIdentity> AuthIdentities => _authIdentities ??= new BaseRepository<AuthIdentity>(_context);
        public IRepository<Profile> Profiles => _profiles ??= new BaseRepository<Profile>(_context);
        public IRepository<PasswordResetToken> PasswordResetTokens => _passwordResetTokens ??= new BaseRepository<PasswordResetToken>(_context);
        public IRepository<Integration> Integrations => _integrations ??= new BaseRepository<Integration>(_context);
        public IRepository<SecurityLog> SecurityLogs => _securityLogs ??= new BaseRepository<SecurityLog>(_context);
        public IRepository<AppLog> AppLogs => _appLogs ??= new BaseRepository<AppLog>(_context);

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