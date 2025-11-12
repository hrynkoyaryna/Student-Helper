using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Notifications;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Notifications;

public sealed class UpdateNotificationSettingsCommandHandler
    : IRequestHandler<UpdateNotificationSettingsCommand>
{
    private readonly INotificationSettingService _service;

    public UpdateNotificationSettingsCommandHandler(INotificationSettingService service)
    {
        _service = service;
    }

    public async Task<Unit> Handle(UpdateNotificationSettingsCommand r, CancellationToken ct)
    {
        var dto = new NotificationSettingDto(
            r.UserId,
            r.EmailEnabled,
            r.TelegramEnabled,
            r.ReminderMinutesBefore
        );

        await _service.UpdateAsync(dto, ct);
        return Unit.Value;
    }
}
