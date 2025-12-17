namespace StudentHelper.BLL.DTOs;

public sealed record GroupAcademicDto(
    int Id,
    string Code,
    string Faculty,
    string Degree,
    int Year
);
