using Microsoft.EntityFrameworkCore;
using DAL.Interfaces;
using DAL.Models;
using DAL.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class NotificationRepository : BaseRepository<NotificationSetting>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }

        public async Task<NotificationSetting?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(ns => ns.UserId == userId);
        }

        public async Task<IEnumerable<ScheduledNotification>> GetPendingNotificationsAsync()
        {
            return await _context.ScheduledNotifications
                .Where(sn => sn.Status == "pending" && sn.ScheduledFor <= DateTime.UtcNow)
                .Include(sn => sn.User)
                .OrderBy(sn => sn.ScheduledFor)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduledNotification>> GetUserNotificationsAsync(int userId)
        {
            return await _context.ScheduledNotifications
                .Where(sn => sn.UserId == userId)
                .OrderByDescending(sn => sn.CreatedAt)
                .ToListAsync();
        }
    }
}