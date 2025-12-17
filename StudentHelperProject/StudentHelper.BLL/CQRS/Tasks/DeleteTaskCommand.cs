using MediatR;

namespace StudentHelper.BLL.CQRS.Tasks;

public sealed record DeleteTaskCommand(int Id) : IRequest<Unit>;
