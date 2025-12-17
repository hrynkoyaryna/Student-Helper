using Xunit;
using Moq;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;
using DAL.Interfaces;
// using DAL.Models; // <-- НЕ ПІДКЛЮЧАЙ ЦЕЙ NAMESPACE ГЛОБАЛЬНО, ЩОБ НЕ БУЛО КОНФЛІКТУ
using Task = System.Threading.Tasks.Task; // Аліас для асинхронності

namespace StudentHelper.Test.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        // Припускаю ITaskRepository або IRepository<DAL.Models.Task>
        private readonly Mock<ITaskRepository> _mockRepo; 
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ITaskRepository>();

            _mockUnitOfWork.Setup(u => u.Tasks).Returns(_mockRepo.Object);

            _service = new TaskService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetByStatusAsync_ShouldFilterOverdueTasks_Correctly()
        {
            // --- Arrange ---
            int userId = 1;
            var now = DateTime.UtcNow;

            var tasks = new List<DAL.Models.Task>
            {
                // Прострочена задача (вчора)
                new DAL.Models.Task { Id = 1, UserId = userId, Status = "new", DueAt = now.AddDays(-1), Title = "Overdue Task" },
                // Актуальна задача (завтра)
                new DAL.Models.Task { Id = 2, UserId = userId, Status = "new", DueAt = now.AddDays(1), Title = "Future Task" },
                // Виконана задача
                new DAL.Models.Task { Id = 3, UserId = userId, Status = "done", DueAt = now.AddDays(-1), Title = "Done Task" }
            };

            _mockRepo.Setup(r => r.GetUserTasksAsync(userId)).ReturnsAsync(tasks);

            // --- Act ---
            // Просимо тільки прострочені ("overdue")
            var result = await _service.GetByStatusAsync(userId, "overdue");

            // --- Assert ---
            Assert.Single(result); // Має бути тільки одна задача
            Assert.Equal("Overdue Task", result[0].Title);
        }

        [Fact]
        public async Task CreateAsync_ShouldSetDefaultCategory_WhenNull()
        {
            // --- Arrange ---
            var dto = new TaskDto(0, 1, 0, "Buy Milk", "Description", DateTime.Now, "new", "High", null); // Category is null

            // --- Act ---
            await _service.CreateAsync(dto);

            // --- Assert ---
            _mockRepo.Verify(r => r.AddAsync(It.Is<DAL.Models.Task>(t => 
                t.Title == "Buy Milk" &&
                t.Category == "Особисте" // Перевіряємо дефолтну категорію
            )), Times.Once);
            
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

    }
}