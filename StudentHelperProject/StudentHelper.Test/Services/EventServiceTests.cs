using Xunit;
using Moq;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;
using DAL.Interfaces;
using DAL.Models;
// Рядок, що фіксить конфлікт імен Task
using Task = System.Threading.Tasks.Task; 

namespace StudentHelper.Test.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        // УВАГА: Якщо тут впаде, перевір чи правильна назва інтерфейсу (може бути IRepository<Event>)
        private readonly Mock<IEventRepository> _mockEventRepository; 
        private readonly EventService _service;

        public EventServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockEventRepository = new Mock<IEventRepository>();

            // ЗВ'ЯЗУЄМО: UnitOfWork повертає наш репозиторій подій
            _mockUnitOfWork.Setup(u => u.Events).Returns(_mockEventRepository.Object);

            _service = new EventService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldMapDtoToEntity_AndSaveChanges()
        {
            // --- Arrange ---
            var dto = new EventDto(
                Id: 0, 
                UserId: 1, 
                SubjectId: 10, 
                LecturerId: 5, 
                RoomId: 3,
                Title: "Math Exam", 
                Description: "Hard exam", 
                StartAt: DateTime.Now, 
                EndAt: DateTime.Now.AddHours(2), 
                EventType: "Exam", 
                RecurrenceRule: null, 
                SourceId: null
            );

            // --- Act ---
            await _service.CreateAsync(dto);

            // --- Assert ---
            
            // 1. Перевіряємо AddAsync (БЕЗ CancellationToken)
            _mockEventRepository.Verify(
                repo => repo.AddAsync(It.Is<Event>(e => 
                    e.Title == dto.Title && 
                    e.UserId == dto.UserId
                )), 
                Times.Once);

            // 2. Перевіряємо SaveChangesAsync (БЕЗ CancellationToken)
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenEventExists_ShouldRemoveAndSave()
        {
            // --- Arrange ---
            int eventId = 99;
            var existingEvent = new Event { Id = eventId, Title = "Party" };

            // Налаштовуємо GetByIdAsync (БЕЗ CancellationToken)
            _mockEventRepository
                .Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(existingEvent);

            // --- Act ---
            await _service.DeleteAsync(eventId);

            // --- Assert ---
            _mockEventRepository.Verify(repo => repo.Remove(existingEvent), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SECURITY_RISK_DeleteAsync_AllowsDeleting_OtherPeoplesEvents()
        {
            // --- ARRANGE ---
            int victimUserId = 100; 
            int eventId = 55; 

            var victimEvent = new Event 
            { 
                Id = eventId, 
                UserId = victimUserId, 
                Title = "Final Thesis Defense" 
            };

            // Налаштовуємо GetByIdAsync (БЕЗ CancellationToken)
            _mockEventRepository
                .Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(victimEvent);

            // --- ACT ---
            await _service.DeleteAsync(eventId);

            // --- ASSERT ---
            _mockEventRepository.Verify(repo => repo.Remove(victimEvent), Times.Once);
        }
    }
}