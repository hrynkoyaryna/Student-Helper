using Xunit;
using Moq;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;
using DAL.Interfaces;
using DAL.Models;
using Task = System.Threading.Tasks.Task;

namespace StudentHelper.Test.Services
{
    public class NotificationSettingServiceTests
    {
        private readonly Mock<INotificationRepository> _mockRepo;
        private readonly NotificationSettingService _service;

        public NotificationSettingServiceTests()
        {
            _mockRepo = new Mock<INotificationRepository>();
            _service = new NotificationSettingService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetByUserIdAsync_WhenSettingExists_ShouldReturnDto()
        {
            // --- Arrange ---
            int userId = 10;
            var setting = new NotificationSetting
            {
                UserId = userId,
                EmailEnabled = true,
                TelegramConnected = false,
                RemindBeforeMinutes = new[] { 60 } // Нагадати за годину
            };

            _mockRepo.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(setting);

            // --- Act ---
            var result = await _service.GetByUserIdAsync(userId);

            // --- Assert ---
            Assert.NotNull(result);
            Assert.True(result.EmailEnabled);
            Assert.Equal(60, result.ReminderMinutesBefore); // Перевіряємо логіку GetReminderMinutes
        }

        [Fact]
        public async Task UpdateAsync_WhenSettingDoesNotExist_ShouldCreateNew()
        {
            // --- Arrange ---
            var dto = new NotificationSettingDto(99, true, true, 30);

            // Репозиторій каже: "Я нічого не знайшов" (null)
            _mockRepo.Setup(r => r.GetByUserIdAsync(dto.UserId))
                     .ReturnsAsync((NotificationSetting?)null);

            // --- Act ---
            await _service.UpdateAsync(dto);

            // --- Assert ---
            // Перевіряємо, що викликали AddAsync (створення)
            _mockRepo.Verify(r => r.AddAsync(It.Is<NotificationSetting>(s => 
                s.UserId == dto.UserId &&
                s.PushEnabled == true && // Дефолтне значення з коду
                s.Timezone == "UTC"      // Дефолтне значення з коду
            )), Times.Once);
            
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SECURITY_RISK_UpdateAsync_AllowsDisablingNotifications_ForOthers()
        {
            // --- Arrange ---
            int victimUserId = 50;
            var victimSettings = new NotificationSetting
            {
                UserId = victimUserId,
                EmailEnabled = true, // Жертва хоче отримувати листи
                TelegramConnected = true
            };

            // Атакуючий хоче вимкнути все для жертви
            var maliciousDto = new NotificationSettingDto(victimUserId, false, false, 0);

            _mockRepo.Setup(r => r.GetByUserIdAsync(victimUserId))
                     .ReturnsAsync(victimSettings);

            // --- Act ---
            // Сервіс сліпо приймає ID з DTO і оновлює запис
            await _service.UpdateAsync(maliciousDto);

            // --- Assert ---
            // Перевіряємо, що налаштування жертви були змінені
            Assert.False(victimSettings.EmailEnabled); // Листи вимкнено!
            Assert.False(victimSettings.TelegramConnected); // Телеграм вимкнено!
            
            _mockRepo.Verify(r => r.Update(victimSettings), Times.Once);
        }
    }
}