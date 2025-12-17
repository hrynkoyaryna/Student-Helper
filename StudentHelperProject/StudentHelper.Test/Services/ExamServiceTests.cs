using Xunit;
using Moq;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;
using DAL.Interfaces;
using DAL.Models;
// ФІКС КОНФЛІКТУ ІМЕН:
using Task = System.Threading.Tasks.Task;

namespace StudentHelper.Test.Services
{
    public class ExamServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        // Припускаю, що інтерфейс називається IExamRepository (аналогічно до Events)
        // Якщо код підсвітить червоним - зміни на IRepository<Exam>
        private readonly Mock<IExamRepository> _mockExamRepository;
        private readonly ExamService _service;

        public ExamServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockExamRepository = new Mock<IExamRepository>();

            // ЗВ'ЯЗУЄМО: UnitOfWork.Exams повертає наш мок
            _mockUnitOfWork.Setup(u => u.Exams).Returns(_mockExamRepository.Object);

            _service = new ExamService(_mockUnitOfWork.Object);
        }

[Fact]
        public async Task CreateAsync_ShouldSaveExam_WithCorrectData()
        {
            // --- Arrange ---
            var dto = new ExamDto(
                Id: 0,
                UserId: 10,
                SubjectId: 5,
                Title: "Cybersecurity Basics",
                ExamDate: DateTime.Now.AddDays(7), // Дата іспиту (DateTime)
                
                // ВИПРАВЛЕННЯ ТУТ:
                // Передаємо час доби (TimeSpan), а не дату
                StartTime: TimeSpan.FromHours(9),  // 09:00:00
                EndTime: TimeSpan.FromHours(12),   // 12:00:00
                
                Description: "Don't forget to study IDOR"
            );

            // --- Act ---
            await _service.CreateAsync(dto);

            // --- Assert ---
            _mockExamRepository.Verify(
                repo => repo.AddAsync(It.Is<Exam>(e =>
                    e.Title == dto.Title &&
                    e.UserId == dto.UserId &&
                    e.SubjectId == dto.SubjectId
                )),
                Times.Once);

            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

    
    }
}