using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.CQRS.Queries;
using StudentHelper.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StudentHelper.BLL.CQRS.Handlers
{
    iusing MediatR;
using BLL.Abstractions;
using BLL.CQRS.Queries;
using BLL.DTOs;

public sealed class GetUserByIdQueryHandler(IUserService svc)
  : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        public Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
            => svc.GetByIdAsync(request.Id, ct);
    }
}
