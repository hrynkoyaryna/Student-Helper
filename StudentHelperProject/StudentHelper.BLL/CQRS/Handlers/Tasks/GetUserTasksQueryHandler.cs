using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Tasks;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Tasks;

public sealed class GetUserTasksQueryHandler
    : IRequestHandler<GetUserTasksQuery, List<TaskDto>>
{
    private readonly ITaskService _service;

    public GetUserTasksQueryHandler(ITaskService service)
    {
        _service = service;
    }

    public Task<List<TaskDto>> Handle(GetUserTasksQuery request, CancellationToken cancellationToken)
        => _service.GetUserTasksAsync(request.UserId, cancellationToken);
}
