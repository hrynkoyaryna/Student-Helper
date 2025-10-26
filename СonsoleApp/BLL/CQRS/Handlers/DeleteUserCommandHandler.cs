using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHelper.BLL.CQRS.Handlers
{
    using MediatR;
    using BLL.Abstractions;
    using BLL.CQRS.Commands;

    public sealed class DeleteUserCommandHandler(IUserService svc)
      : IRequestHandler<DeleteUserCommand>
    {
        public async Task<Unit> Handle(DeleteUserCommand r, CancellationToken ct)
        {
            await svc.DeleteAsync(r.Id, ct);
            return Unit.Value;
        }
    }
}
