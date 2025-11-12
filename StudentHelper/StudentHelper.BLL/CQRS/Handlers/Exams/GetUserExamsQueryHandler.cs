using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Exams;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Exams;

public sealed class GetUserExamsQueryHandler
    : IRequestHandler<GetUserExamsQuery, List<ExamDto>>
{
    private readonly IExamService _service;

    public GetUserExamsQueryHandler(IExamService service)
    {
        _service = service;
    }

    public Task<List<ExamDto>> Handle(GetUserExamsQuery request, CancellationToken cancellationToken)
        => _service.GetUserExamsAsync(request.UserId, cancellationToken);
}
