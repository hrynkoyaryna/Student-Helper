using Xunit;
using Moq;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;
using StudentHelper.BLL.Abstractions; // Для IEmailService
using DAL.Interfaces;
using DAL.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace StudentHelper.Test.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockMediator = new Mock<IMediator>();
            _mockEmailService = new Mock<IEmailService>();
            _mockLogger = new Mock<ILogger<UserService>>();

            // Зв'язуємо UnitOfWork з UserRepo
            _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepo.Object);

            _service = new UserService(
                _mockUnitOfWork.Object, 
                _mockMediator.Object, 
                _mockEmailService.Object, 
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldVerifyPassword_UsingBCrypt()
        {
            // --- Arrange ---
            string email = "hacker@test.com";
            string rawPassword = "password123";
            // Створюємо правильний хеш для пароля
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

            var user = new User
            {
                Id = 1,
                Email = email,
                PasswordHash = hashedPassword, // У базі лежить хеш
                FirstName = "Neo",
                LastName = "Anderson"
            };

            _mockUserRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            // --- Act ---
            var result = await _service.AuthenticateAsync(email, rawPassword);

            // --- Assert ---
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }
    }
}