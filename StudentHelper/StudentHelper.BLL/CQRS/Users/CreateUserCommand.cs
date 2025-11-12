// StudentHelper.BLL/CQRS/Users/CreateUserCommand.cs
using MediatR;

namespace StudentHelper.BLL.CQRS.Users
{
    public record CreateUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password
    ) : IRequest<int>;
}