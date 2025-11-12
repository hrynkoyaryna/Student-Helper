using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Tasks;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Tasks;

public sealed class GetUserTasksByStatusQueryHandler
    : IRequestHandler<GetUserTasksByStatusQuery, List<TaskDto>>
{
    private readonly ITaskService _service;

    public GetUserTasksByStatusQueryHandler(ITaskService service)
    {
        _service = service;
    }

    public Task<List<TaskDto>> Handle(GetUserTasksByStatusQuery request, CancellationToken cancellationToken)
        => _service.GetByStatusAsync(request.UserId, request.Status, cancellationToken);
}
