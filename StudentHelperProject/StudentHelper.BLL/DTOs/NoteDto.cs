namespace StudentHelper.BLL.DTOs;

public sealed record NoteDto(
    int Id,
    int UserId,
    string Title,
    string Content,
    bool IsPinned,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
