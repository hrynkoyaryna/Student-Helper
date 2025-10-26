using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHelper.BLL.CQRS.Queries
{
    using BLL.DTOs;
    using MediatR;

    public sealed record GetUsersQuery() : IRequest<List<UserDto>>;
}
