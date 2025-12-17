using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Groups;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Groups;

public sealed class GetGroupByCodeQueryHandler
    : IRequestHandler<GetGroupByCodeQuery, GroupAcademicDto?>
{
    private readonly IGroupAcademicService _service;

    public GetGroupByCodeQueryHandler(IGroupAcademicService service)
    {
        _service = service;
    }

    public Task<GroupAcademicDto?> Handle(GetGroupByCodeQuery request, CancellationToken ct)
        => _service.GetByCodeAsync(request.Code, ct);
}
