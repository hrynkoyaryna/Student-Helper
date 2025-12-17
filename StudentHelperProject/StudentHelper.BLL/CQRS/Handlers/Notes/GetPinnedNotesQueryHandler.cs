using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Notes;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Notes;

public sealed class GetPinnedNotesQueryHandler
    : IRequestHandler<GetPinnedNotesQuery, List<NoteDto>>
{
    private readonly INoteService _service;

    public GetPinnedNotesQueryHandler(INoteService service)
    {
        _service = service;
    }

    public Task<List<NoteDto>> Handle(GetPinnedNotesQuery request, CancellationToken ct)
        => _service.GetPinnedNotesAsync(request.UserId, ct);
}
