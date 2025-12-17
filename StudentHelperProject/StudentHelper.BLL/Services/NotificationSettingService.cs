using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class NotificationSettingService : INotificationSettingService
{
    private readonly INotificationRepository _repo;
    private readonly ILogger<NotificationSettingService> _logger;

    public NotificationSettingService(INotificationRepository repo, ILogger<NotificationSettingService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<NotificationSettingDto?> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        _logger.LogInformation("Fetching notification settings for user {UserId}", userId);
        try
        {
            var setting = await _repo.GetByUserIdAsync(userId);
            if (setting is null)
            {
                _logger.LogWarning("Notification settings not found for user {UserId}", userId);
                return null;
            }

            var dto = new NotificationSettingDto(
                setting.UserId,
                setting.EmailEnabled,
                setting.TelegramConnected,
                GetReminderMinutes(setting)
            );
            _logger.LogInformation("Notification settings retrieved for user {UserId} - Email: {EmailEnabled}, Telegram: {TelegramConnected}",
                userId, setting.EmailEnabled, setting.TelegramConnected);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching notification settings for user {UserId}", userId);
            throw;
        }
    }

    public async Task UpdateAsync(NotificationSettingDto dto, CancellationToken ct = default)
    {
        _logger.LogInformation("Updating notification settings for user {UserId}", dto.UserId);
        try
        {
            var setting = await _repo.GetByUserIdAsync(dto.UserId);

            if (setting is null)
            {
                _logger.LogInformation("Creating new notification settings for user {UserId}", dto.UserId);
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
                await _repo.AddAsync(setting);
                _logger.LogInformation("New notification settings created for user {UserId}", dto.UserId);
            }
            else
            {
                _logger.LogInformation("Updating existing notification settings for user {UserId}", dto.UserId);
                setting.EmailEnabled = dto.EmailEnabled;
                setting.TelegramConnected = dto.TelegramEnabled;
                setting.RemindBeforeMinutes = ConvertToMinutesArray(dto.ReminderMinutesBefore);
                _repo.Update(setting);
            }

            await _repo.SaveChangesAsync();
            _logger.LogInformation("Notification settings updated successfully for user {UserId} - Email: {EmailEnabled}, Telegram: {TelegramEnabled}",
                dto.UserId, dto.EmailEnabled, dto.TelegramEnabled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification settings for user {UserId}", dto.UserId);
            throw;
        }
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