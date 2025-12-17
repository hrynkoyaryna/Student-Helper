using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Tasks;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Tasks;

public sealed class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, int>
{
    private readonly ITaskService _service;

    public CreateTaskCommandHandler(ITaskService service)
    {
        _service = service;
    }

    public Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var dto = new TaskDto(
            Id: 0,
            UserId: request.UserId,
            SubjectId: request.SubjectId,
            Title: request.Title,
            Description: request.Description,
            DueDate: request.DueDate,
            Status: "current",
            Priority: request.Priority
        );

        return _service.CreateAsync(dto, cancellationToken);
    }
}
