using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHelper.Tests.BLL
{
    using BLL.DTOs;
    using BLL.Services;
    using DAL.Interfaces;
    using DAL.Models;
    using Moq;
    using StudentHelper.BLL.DTOs;
    using StudentHelper.BLL.Services;

    public class UserServiceTests
    {
        [Fact]
        public async Task CreateAsync_ReturnsNewId()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback<User, CancellationToken>((u, _) => u.Id = 77)
                .Returns(Task.CompletedTask);
            repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var svc = new UserService(repo.Object);
            var id = await svc.CreateAsync(new UserDto(0, "Alice", "a@a.com"));

            Assert.Equal(77, id);
        }

        [Fact]
        public async Task GetAllAsync_MapsToDto()
        {
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([new User { Id = 1, Name = "Bob", Email = "b@b.com" }]);

            var svc = new UserService(repo.Object);
            var list = await svc.GetAllAsync();

            Assert.Single(list);
            Assert.Equal("Bob", list[0].Name);
        }
    }

}
