using MediatR;

namespace StudentHelper.BLL.CQRS.Tasks;

public sealed record UpdateTaskCommand(
    int Id,
    int UserId,
    int? SubjectId,
    string Title,
    string? Description,
    DateTime? DueDate,
    string Status,
    string? Priority
) : IRequest;
