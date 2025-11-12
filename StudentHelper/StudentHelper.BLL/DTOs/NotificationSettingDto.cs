namespace StudentHelper.BLL.DTOs;

public sealed record NotificationSettingDto(
    int UserId,
    bool EmailEnabled,
    bool TelegramEnabled,
    int ReminderMinutesBefore
);
