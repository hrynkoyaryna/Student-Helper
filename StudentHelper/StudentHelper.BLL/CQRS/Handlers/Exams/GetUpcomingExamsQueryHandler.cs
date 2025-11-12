using MediatR;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Exams;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Handlers.Exams;

public sealed class GetUpcomingExamsQueryHandler
    : IRequestHandler<GetUpcomingExamsQuery, List<ExamDto>>
{
    private readonly IExamService _service;

    public GetUpcomingExamsQueryHandler(IExamService service)
    {
        _service = service;
    }

    public Task<List<ExamDto>> Handle(GetUpcomingExamsQuery request, CancellationToken cancellationToken)
        => _service.GetUpcomingExamsAsync(request.UserId, request.DaysAhead, cancellationToken);
}
