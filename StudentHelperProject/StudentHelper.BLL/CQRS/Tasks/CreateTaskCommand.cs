using MediatR;

namespace StudentHelper.BLL.CQRS.Tasks;

public sealed record CreateTaskCommand(
    int UserId,
    int? SubjectId,
    string Title,
    string? Description,
    DateTime? DueDate,
    string? Priority
) : IRequest<int>;
