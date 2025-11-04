//для NotificationSetting та ScheduledNotification
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface INotificationRepository : IRepository<NotificationSetting>
    {
        Task<NotificationSetting?> GetByUserIdAsync(int userId);
    }
}