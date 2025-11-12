using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Subjects;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Subjects;

public sealed class GetAllSubjectsQueryHandler
    : IRequestHandler<GetAllSubjectsQuery, List<SubjectDto>>
{
    private readonly ISubjectService _service;

    public GetAllSubjectsQueryHandler(ISubjectService service)
    {
        _service = service;
    }

    public Task<List<SubjectDto>> Handle(GetAllSubjectsQuery request, CancellationToken ct)
        => _service.GetAllAsync(ct);
}
