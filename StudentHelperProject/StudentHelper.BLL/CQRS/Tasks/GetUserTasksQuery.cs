using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Tasks;

public sealed record GetUserTasksQuery(int UserId) : IRequest<List<TaskDto>>;
