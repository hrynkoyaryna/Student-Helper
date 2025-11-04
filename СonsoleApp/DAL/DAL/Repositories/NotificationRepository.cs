using Microsoft.EntityFrameworkCore;
using DAL.Interfaces;
using DAL.Models;
using DAL.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; 

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
            var now = DateTime.UtcNow;
            
            return await _context.Set<ScheduledNotification>()
                .Where(sn => sn.Status == "pending" && sn.FireAt <= now)
                .Include(sn => sn.User)
                .OrderBy(sn => sn.FireAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduledNotification>> GetUserNotificationsAsync(int userId)
        {
            return await _context.Set<ScheduledNotification>()
                .Where(sn => sn.UserId == userId)
                .OrderByDescending(sn => sn.FireAt)
                .ToListAsync();
        }
    }
}