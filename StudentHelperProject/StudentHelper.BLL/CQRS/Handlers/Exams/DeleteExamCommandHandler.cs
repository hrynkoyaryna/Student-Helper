using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Exams;

namespace StudentHelper.BLL.CQRS.Handlers.Exams;

public sealed class DeleteExamCommandHandler
    : IRequestHandler<DeleteExamCommand, Unit>
{
    private readonly IExamService _service;

    public DeleteExamCommandHandler(IExamService service)
    {
        _service = service;
    }

    public async Task<Unit> Handle(DeleteExamCommand request, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(request.Id, cancellationToken);
        return Unit.Value;
    }
}
