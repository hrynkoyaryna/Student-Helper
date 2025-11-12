using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Notes;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Notes;

public sealed class CreateNoteCommandHandler
    : IRequestHandler<CreateNoteCommand, int>
{
    private readonly INoteService _service;

    public CreateNoteCommandHandler(INoteService service)
    {
        _service = service;
    }

    public Task<int> Handle(CreateNoteCommand r, CancellationToken ct)
    {
        var dto = new NoteDto(
            Id: 0,
            UserId: r.UserId,
            Title: r.Title,
            Content: r.Content,
            IsPinned: r.IsPinned,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow
        );

        return _service.CreateAsync(dto, ct);
    }
}
