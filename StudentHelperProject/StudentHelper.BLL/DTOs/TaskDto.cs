namespace StudentHelper.BLL.DTOs;

public sealed record TaskDto(
    int Id,
    int UserId,
    int? SubjectId,
    string Title,
    string? Description,
    DateTime? DueDate,
    string Status,
    string? Priority,
    string? Category = "Особисте"
);
