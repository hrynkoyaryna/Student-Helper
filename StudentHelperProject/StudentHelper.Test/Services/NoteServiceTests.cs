using Xunit;
using Moq;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;
using DAL.Interfaces;
using DAL.Models;
// Аліас обов'язковий
using Task = System.Threading.Tasks.Task;

namespace StudentHelper.Test.Services
{
    public class NoteServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        // Припускаю назву INoteRepository. Якщо червоне - спробуй IRepository<Note>
        private readonly Mock<INoteRepository> _mockNoteRepository; 
        private readonly NoteService _service;

        public NoteServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockNoteRepository = new Mock<INoteRepository>();

            _mockUnitOfWork.Setup(u => u.Notes).Returns(_mockNoteRepository.Object);

            _service = new NoteService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldSaveNote_WithDefaultDates()
        {
            // --- Arrange ---
            var dto = new NoteDto(0, 1, "Secret", "My passwords", true, DateTime.Now, DateTime.Now);

            // --- Act ---
            await _service.CreateAsync(dto);

            // --- Assert ---
            _mockNoteRepository.Verify(
                repo => repo.AddAsync(It.Is<Note>(n => 
                    n.Title == "Secret" && 
                    n.IsPinned == true
                )), 
                Times.Once);

            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SECURITY_RISK_UpdateAsync_AllowsOverwriting_OtherPeoplesNotes()
        {
            // --- Arrange ---
            int noteId = 50;
            int victimUserId = 10;
            
            // Оригінальна нотатка жертви
            var victimNote = new Note 
            { 
                Id = noteId, 
                UserId = victimUserId, 
                Content = "Important Data" 
            };

            // ДТО атакуючого (змінює текст на "Hacked")
            var attackerDto = new NoteDto(noteId, 999, "Hacked Title", "HACKED CONTENT", false, DateTime.Now, DateTime.Now);

            // Мокаємо існування нотатки
            _mockNoteRepository.Setup(r => r.GetByIdAsync(noteId)).ReturnsAsync(victimNote);

            // --- Act ---
            // Атакуючий викликає Update. Сервіс НЕ перевіряє, чи співпадає UserId жертви і атакуючого.
            await _service.UpdateAsync(attackerDto);

            // --- Assert ---
            // Перевіряємо, що в базі зберігся змінений об'єкт
            Assert.Equal("HACKED CONTENT", victimNote.Content);
            
            // Перевіряємо, що викликали Update
            _mockNoteRepository.Verify(r => r.Update(victimNote), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}