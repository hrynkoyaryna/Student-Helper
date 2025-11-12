using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Notifications;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Notifications;

public sealed class GetNotificationSettingsQueryHandler
    : IRequestHandler<GetNotificationSettingsQuery, NotificationSettingDto?>
{
    private readonly INotificationSettingService _service;

    public GetNotificationSettingsQueryHandler(INotificationSettingService service)
    {
        _service = service;
    }

    public Task<NotificationSettingDto?> Handle(GetNotificationSettingsQuery request, CancellationToken ct)
        => _service.GetByUserIdAsync(request.UserId, ct);
}
