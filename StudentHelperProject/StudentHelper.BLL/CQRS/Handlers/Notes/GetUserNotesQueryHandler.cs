using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Notes;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Notes;

public sealed class GetUserNotesQueryHandler
    : IRequestHandler<GetUserNotesQuery, List<NoteDto>>
{
    private readonly INoteService _service;

    public GetUserNotesQueryHandler(INoteService service)
    {
        _service = service;
    }

    public Task<List<NoteDto>> Handle(GetUserNotesQuery request, CancellationToken ct)
        => _service.GetUserNotesAsync(request.UserId, ct);
}
