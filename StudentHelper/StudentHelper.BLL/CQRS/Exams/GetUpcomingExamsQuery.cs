using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Exams;

public sealed record GetUpcomingExamsQuery(int UserId, int DaysAhead)
    : IRequest<List<ExamDto>>;
