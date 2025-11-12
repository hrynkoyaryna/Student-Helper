using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Subjects;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Subjects;

public sealed class GetSubjectByShortNameQueryHandler
    : IRequestHandler<GetSubjectByShortNameQuery, SubjectDto?>
{
    private readonly ISubjectService _service;

    public GetSubjectByShortNameQueryHandler(ISubjectService service)
    {
        _service = service;
    }

    public Task<SubjectDto?> Handle(GetSubjectByShortNameQuery request, CancellationToken ct)
        => _service.GetByShortNameAsync(request.ShortName, ct);
}
