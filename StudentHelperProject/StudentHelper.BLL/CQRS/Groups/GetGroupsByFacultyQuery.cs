using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Groups;

public sealed record GetGroupsByFacultyQuery(string Faculty) : IRequest<List<GroupAcademicDto>>;
