using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHelper.BLL.CQRS.Commands
{
    using MediatR;

    public sealed record DeleteUserCommand(int Id) : IRequest;
}
