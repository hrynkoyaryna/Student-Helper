using MediatR;

namespace StudentHelper.BLL.CQRS.Notifications;

public sealed record UpdateNotificationSettingsCommand(
    int UserId,
    bool EmailEnabled,
    bool TelegramEnabled,
    int ReminderMinutesBefore
) : IRequest;
