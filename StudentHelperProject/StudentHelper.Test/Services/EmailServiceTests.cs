using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.Configuration;

namespace StudentHelper.Test.Services
{
    public class EmailServiceTests
    {
        private readonly Mock<IOptions<EmailSettings>> _mockOptions;
        private readonly Mock<ILogger<EmailService>> _mockLogger;

        public EmailServiceTests()
        {
            _mockOptions = new Mock<IOptions<EmailSettings>>();
            _mockLogger = new Mock<ILogger<EmailService>>();
        }

        [Fact]
        public void Constructor_WhenSettingsAreNull_ShouldLogErrorMessage()
        {
       
            _mockOptions.Setup(opt => opt.Value).Returns((EmailSettings?)null);

            var service = new EmailService(_mockOptions.Object, _mockLogger.Object);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
            
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("EmailSettings is null")), 

                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public void Constructor_WhenSettingsAreValid_ShouldLogInitializationInfo()
        {
            var settings = new EmailSettings 
            { 
                SmtpServer = "smtp.test.com", 
                SmtpPort = 587,
                SenderEmail = "admin@test.com",
   
                SenderPassword = "dummy",
                EnableSsl = true
            };
            
            _mockOptions.Setup(opt => opt.Value).Returns(settings);

            var service = new EmailService(_mockOptions.Object, _mockLogger.Object);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("EmailService initialized")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}