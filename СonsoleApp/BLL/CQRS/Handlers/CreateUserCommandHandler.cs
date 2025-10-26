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

    public sealed class CreateUserCommandHandler(IUserService svc)
      : IRequestHandler<CreateUserCommand, int>
    {
        public Task<int> Handle(CreateUserCommand r, CancellationToken ct)
            => svc.CreateAsync(new UserDto(0, r.Name, r.Email), ct);
    }
}
