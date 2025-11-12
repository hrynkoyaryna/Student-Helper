using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Groups;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Groups;

public sealed class GetGroupsByFacultyQueryHandler
    : IRequestHandler<GetGroupsByFacultyQuery, List<GroupAcademicDto>>
{
    private readonly IGroupAcademicService _service;

    public GetGroupsByFacultyQueryHandler(IGroupAcademicService service)
    {
        _service = service;
    }

    public Task<List<GroupAcademicDto>> Handle(GetGroupsByFacultyQuery request, CancellationToken ct)
        => _service.GetByFacultyAsync(request.Faculty, ct);
}
