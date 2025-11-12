using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Abstractions;

public interface INotificationSettingService
{
    Task<NotificationSettingDto?> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task UpdateAsync(NotificationSettingDto dto, CancellationToken ct = default);
}
