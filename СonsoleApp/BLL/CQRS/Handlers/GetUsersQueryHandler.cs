using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHelper.BLL.CQRS.Handlers
{
    using MediatR;
    using BLL.Abstractions;
    using BLL.CQRS.Queries;
    using BLL.DTOs;

    public sealed class GetUsersQueryHandler(IUserService svc)
      : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        public Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
            => svc.GetAllAsync(ct);
    }
}
