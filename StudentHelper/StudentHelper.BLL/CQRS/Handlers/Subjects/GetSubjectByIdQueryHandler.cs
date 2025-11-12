using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Subjects;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Subjects;

public sealed class GetSubjectByIdQueryHandler
    : IRequestHandler<GetSubjectByIdQuery, SubjectDto?>
{
    private readonly ISubjectService _service;

    public GetSubjectByIdQueryHandler(ISubjectService service)
    {
        _service = service;
    }

    public Task<SubjectDto?> Handle(GetSubjectByIdQuery request, CancellationToken ct)
        => _service.GetByIdAsync(request.Id, ct);
}
