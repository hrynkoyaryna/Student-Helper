using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Groups;

public sealed record GetGroupByCodeQuery(string Code) : IRequest<GroupAcademicDto?>;
