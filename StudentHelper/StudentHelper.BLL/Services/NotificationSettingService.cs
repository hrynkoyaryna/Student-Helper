using DAL.Interfaces;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class NotificationSettingService : INotificationSettingService
{
    private readonly INotificationRepository _repo;

    public NotificationSettingService(INotificationRepository repo)
    {
        _repo = repo;
    }

    public async Task<NotificationSettingDto?> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var setting = await _repo.GetByUserIdAsync(userId);
        if (setting is null) return null;

        return new NotificationSettingDto(
            setting.UserId,
            setting.EmailEnabled,
            setting.TelegramConnected,
            GetReminderMinutes(setting)
        );
    }

    public async Task UpdateAsync(NotificationSettingDto dto, CancellationToken ct = default)
    {
        var setting = await _repo.GetByUserIdAsync(dto.UserId);

        if (setting is null)
        {
            setting = new DAL.Models.NotificationSetting
            {
                UserId = dto.UserId,
                PushEnabled = true,
                RemindBeforeMinutes = ConvertToMinutesArray(dto.ReminderMinutesBefore),
                Timezone = "UTC",
                TelegramChatId = string.Empty,
                EmailEnabled = dto.EmailEnabled,
                TelegramConnected = dto.TelegramEnabled
            };
            await _repo.AddAsync(setting, ct);
        }
        else
        {
            setting.EmailEnabled = dto.EmailEnabled;
            setting.TelegramConnected = dto.TelegramEnabled;
            setting.RemindBeforeMinutes = ConvertToMinutesArray(dto.ReminderMinutesBefore);
        }

        _repo.Update(setting);
        await _repo.SaveChangesAsync(ct);
    }

    private static int GetReminderMinutes(DAL.Models.NotificationSetting setting)
    {
        if (setting.RemindBeforeMinutes == null || setting.RemindBeforeMinutes.Length == 0)
            return 15;

        return setting.RemindBeforeMinutes[0];
    }

    private static int[] ConvertToMinutesArray(int minutes)
    {
        return new int[] { minutes };
    }
}