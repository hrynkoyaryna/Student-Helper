namespace StudentHelper.BLL.DTOs;

public sealed record EventDto(
    int Id,
    int UserId,
    int? SubjectId,
    int? LecturerId,
    int? RoomId,
    string Title,
    string? Description,
    DateTime StartAt,
    DateTime EndAt,
    string EventType,
    string? RecurrenceRule,
    int? SourceId
);
