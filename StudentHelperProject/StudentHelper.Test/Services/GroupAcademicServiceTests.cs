using Xunit;
using Moq;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;
using DAL.Interfaces;
using DAL.Models;
// Обов'язковий аліас для Task
using Task = System.Threading.Tasks.Task;

namespace StudentHelper.Test.Services
{
    public class GroupAcademicServiceTests
    {
        private readonly Mock<IGroupAcademicRepository> _mockRepo;
        private readonly GroupAcademicService _service;

        public GroupAcademicServiceTests()
        {
            _mockRepo = new Mock<IGroupAcademicRepository>();
            _service = new GroupAcademicService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectlyMappedDto()
        {
            // --- Arrange ---
            int groupId = 10;
            var groupEntity = new GroupAcademic
            {
                Id = groupId,
                Code = "CS-101",
                Faculty = "Computer Science", // Важливо перевірити, чи не переплутані поля
                Degree = "Bachelor",
                Year = 1
            };

            // Налаштовуємо мок
            _mockRepo.Setup(r => r.GetByIdAsync(groupId))
                     .ReturnsAsync(groupEntity);

            // --- Act ---
            var result = await _service.GetByIdAsync(groupId);

            // --- Assert ---
            Assert.NotNull(result);
            Assert.Equal(groupId, result.Id);
            Assert.Equal("CS-101", result.Code);
            Assert.Equal("Computer Science", result.Faculty); // Перевіряємо мапінг
            Assert.Equal("Bachelor", result.Degree);
        }

        [Fact]
        public async Task GetByIdAsync_WhenGroupNotFound_ShouldReturnNull()
        {
            // --- Arrange ---
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                     .ReturnsAsync((GroupAcademic?)null);

            // --- Act ---
            var result = await _service.GetByIdAsync(999);

            // --- Assert ---
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByFacultyAsync_ShouldReturnListOfGroups()
        {
            // --- Arrange ---
            string faculty = "Cybersecurity";
            var groups = new List<GroupAcademic>
            {
                new GroupAcademic { Id = 1, Code = "CB-1", Faculty = faculty },
                new GroupAcademic { Id = 2, Code = "CB-2", Faculty = faculty }
            };

            _mockRepo.Setup(r => r.GetGroupsByFacultyAsync(faculty))
                     .ReturnsAsync(groups);

            // --- Act ---
            var result = await _service.GetByFacultyAsync(faculty);

            // --- Assert ---
            Assert.Equal(2, result.Count);
            Assert.Equal("CB-1", result[0].Code);
        }

        /// <summary>
        /// Цей тест демонструє, що будь-хто може отримати інформацію про групу по ID.
        /// Це "Enumeration Risk", але чи це вразливість - залежить від контексту (чи секретні це групи).
        /// </summary>
        
       
    }
}