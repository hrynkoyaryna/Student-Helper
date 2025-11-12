using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Exams;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Exams;

public sealed class UpdateExamCommandHandler
    : IRequestHandler<UpdateExamCommand>
{
    private readonly IExamService _service;

    public UpdateExamCommandHandler(IExamService service)
    {
        _service = service;
    }

    public async Task<Unit> Handle(UpdateExamCommand request, CancellationToken cancellationToken)
    {
        var dto = new ExamDto(
            Id: request.Id,
            UserId: request.UserId,
            SubjectId: request.SubjectId,
            Title: request.Title,
            ExamDate: request.ExamDate,
            StartTime: request.StartTime,
            EndTime: request.EndTime,
            Description: request.Description
        );

        await _service.UpdateAsync(dto, cancellationToken);
        return Unit.Value;
    }
}
