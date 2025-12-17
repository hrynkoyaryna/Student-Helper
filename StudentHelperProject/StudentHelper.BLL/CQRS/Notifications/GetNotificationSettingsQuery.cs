using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Notifications;

public sealed record GetNotificationSettingsQuery(int UserId)
    : IRequest<NotificationSettingDto?>;
