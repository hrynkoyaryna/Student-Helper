using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Tasks;

public sealed record GetUserTasksByStatusQuery(int UserId, string Status)
    : IRequest<List<TaskDto>>;
