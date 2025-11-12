using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Tasks;

namespace StudentHelper.BLL.CQRS.Handlers.Tasks;

public sealed class DeleteTaskCommandHandler
    : IRequestHandler<DeleteTaskCommand>
{
    private readonly ITaskService _service;

    public DeleteTaskCommandHandler(ITaskService service)
    {
        _service = service;
    }

    public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
