namespace StudentHelper.BLL.DTOs;

public sealed record ExamDto(
    int Id,
    int UserId,
    int SubjectId,
    string Title,
    DateTime ExamDate,
    TimeSpan? StartTime,
    TimeSpan? EndTime,
    string? Description
);
