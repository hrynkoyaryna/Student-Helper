using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Exams;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Exams;

public sealed class CreateExamCommandHandler
    : IRequestHandler<CreateExamCommand, int>
{
    private readonly IExamService _service;

    public CreateExamCommandHandler(IExamService service)
    {
        _service = service;
    }

    public Task<int> Handle(CreateExamCommand request, CancellationToken cancellationToken)
    {
        var dto = new ExamDto(
            Id: 0,
            UserId: request.UserId,
            SubjectId: request.SubjectId,
            Title: request.Title,
            ExamDate: request.ExamDate,
            StartTime: request.StartTime,
            EndTime: request.EndTime,
            Description: request.Description
        );

        return _service.CreateAsync(dto, cancellationToken);
    }
}
