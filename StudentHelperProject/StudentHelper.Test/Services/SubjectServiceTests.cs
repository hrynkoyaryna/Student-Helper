using Xunit;
using Moq;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;
using DAL.Interfaces;
using DAL.Models;
using Task = System.Threading.Tasks.Task;

namespace StudentHelper.Test.Services
{
    public class SubjectServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        // Перевір назву: ISubjectRepository або IRepository<Subject>
        private readonly Mock<ISubjectRepository> _mockRepo;
        private readonly SubjectService _service;

        public SubjectServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ISubjectRepository>();

            _mockUnitOfWork.Setup(u => u.Subjects).Returns(_mockRepo.Object);

            _service = new SubjectService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateSubject_WithDefaultColor()
        {
            // --- Arrange ---
            // Колір null, має стати дефолтним синім
            var dto = new SubjectDto(0, "Math", "MA", null); 

            // --- Act ---
            await _service.CreateAsync(dto);

            // --- Assert ---
            _mockRepo.Verify(r => r.AddAsync(It.Is<Subject>(s => 
                s.Name == "Math" && 
                s.DefaultColor == "#3357FF" // Перевіряємо дефолтну логіку
            )), Times.Once);
            
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}