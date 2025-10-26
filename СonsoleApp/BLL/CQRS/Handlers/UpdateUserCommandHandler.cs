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
    using BLL.DTOs;

    public sealed class UpdateUserCommandHandler(IUserService svc)
      : IRequestHandler<UpdateUserCommand>
    {
        public async Task<Unit> Handle(UpdateUserCommand r, CancellationToken ct)
        {
            await svc.UpdateAsync(new UserDto(r.Id, r.Name, r.Email), ct);
            return Unit.Value;
        }
    }
}
