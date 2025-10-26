using Moq;
using BLL.Abstractions;
using BLL.CQRS.Handlers;
using BLL.CQRS.Queries;
using BLL.CQRS.Commands;
using BLL.DTOs;

public class CqrsHandlersTests
{
    [Fact]
    public async Task GetUsersQueryHandler_ReturnsUsers()
    {
        var svc = new Mock<IUserService>();
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync([new UserDto(1, "Tom", "t@t.com")]);

        var h = new GetUsersQueryHandler(svc.Object);
        var res = await h.Handle(new GetUsersQuery(), default);

        Assert.Single(res);
        Assert.Equal("Tom", res[0].Name);
    }

    [Fact]
    public async Task CreateUserCommandHandler_CreatesUser()
    {
        var svc = new Mock<IUserService>();
        svc.Setup(s => s.CreateAsync(It.IsAny<UserDto>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(5);

        var h = new CreateUserCommandHandler(svc.Object);
        var id = await h.Handle(new CreateUserCommand("Nina", "n@n.com"), default);

        Assert.Equal(5, id);
    }
}
