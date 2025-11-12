using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Tasks;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Tasks;

public sealed class UpdateTaskCommandHandler
    : IRequestHandler<UpdateTaskCommand>
{
    private readonly ITaskService _service;

    public UpdateTaskCommandHandler(ITaskService service)
    {
        _service = service;
    }

    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var dto = new TaskDto(
            Id: request.Id,
            UserId: request.UserId,
            SubjectId: request.SubjectId,
            Title: request.Title,
            Description: request.Description,
            DueDate: request.DueDate,
            Status: request.Status,
            Priority: request.Priority
        );

        await _service.UpdateAsync(dto, cancellationToken);
        return Unit.Value;
    }
}
