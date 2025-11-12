using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Notes;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Notes;

public sealed class UpdateNoteCommandHandler
    : IRequestHandler<UpdateNoteCommand>
{
    private readonly INoteService _service;

    public UpdateNoteCommandHandler(INoteService service)
    {
        _service = service;
    }

    public async Task<Unit> Handle(UpdateNoteCommand r, CancellationToken ct)
    {
        var dto = new NoteDto(
            Id: r.Id,
            UserId: r.UserId,
            Title: r.Title,
            Content: r.Content,
            IsPinned: r.IsPinned,
            CreatedAt: DateTime.MinValue,
            UpdatedAt: DateTime.UtcNow
        );

        await _service.UpdateAsync(dto, ct);
        return Unit.Value;
    }
}
