namespace StudentHelper.BLL.DTOs;

public sealed record SubjectDto(
    int Id,
    string Name,
    string ShortName,
    string? ColorHex
);
