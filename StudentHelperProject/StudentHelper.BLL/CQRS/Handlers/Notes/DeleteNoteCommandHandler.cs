using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Notes;

namespace StudentHelper.BLL.CQRS.Handlers.Notes;

public sealed class DeleteNoteCommandHandler
    : IRequestHandler<DeleteNoteCommand, Unit>
{
    private readonly INoteService _service;

    public DeleteNoteCommandHandler(INoteService service)
    {
        _service = service;
    }

    public async Task<Unit> Handle(DeleteNoteCommand r, CancellationToken ct)
    {
        await _service.DeleteAsync(r.Id, ct);
        return Unit.Value;
    }
}
