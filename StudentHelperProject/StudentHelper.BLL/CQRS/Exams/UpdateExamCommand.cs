using MediatR;

namespace StudentHelper.BLL.CQRS.Exams;

public sealed record UpdateExamCommand(
    int Id,
    int UserId,
    int SubjectId,
    string Title,
    DateTime ExamDate,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string? Description
) : IRequest<Unit>;
