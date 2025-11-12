using MediatR;
using StudentHelper.BLL.CQRS.Users;
using StudentHelper.BLL.Services;
using StudentHelper.BLL.DTOs;
using StudentHelper.BLL.CQRS.Tasks;
using StudentHelper.BLL.CQRS.Exams;
using StudentHelper.BLL.CQRS.Notes;

namespace StudentHelper.MAUI.Services
{
    public class AppService : IAppService
    {
        private readonly IUserContext _userContext;
        private readonly IMediator _mediator;
        private readonly IUserService _userService;

        public AppService(
            IUserContext userContext,
            IMediator mediator,
            IUserService userService)
        {
            _userContext = userContext;
            _mediator = mediator;
            _userService = userService;
        }

        public async Task<ServiceResult> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                var user = await _userService.AuthenticateAsync(email, password);

                if (user != null)
                {
                    _userContext.SetCurrentUser(user.Id);
                    return new ServiceResult { IsSuccess = true };
                }

                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Невірний email або пароль"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Помилка автентифікації: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult> AuthenticateWithOpenIdAsync()
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = "OpenID Connect не підтримується в цій версії"
            };
        }

        public async Task<ServiceResult> RegisterUserAsync(string firstName, string lastName, string email, string password)
        {
            try
            {
                var command = new CreateUserCommand(firstName, lastName, email, password);
                var userId = await _mediator.Send(command);

                _userContext.SetCurrentUser(userId);
                return new ServiceResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Помилка реєстрації: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResult> SendPasswordResetCodeAsync(string email)
        {
            try
            {
                var result = await _userService.SendPasswordResetCodeAsync(email);
                return new ServiceResult
                {
                    IsSuccess = result,
                    ErrorMessage = result ? null : "Користувача з таким email не знайдено"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Помилка відновлення пароля: {ex.Message}"
                };
            }
        }

        public async Task LogoutAsync()
        {
            _userContext.SetCurrentUser(0);
        }

        public async Task<object> GetUserEventsAsync()
        {
            // Буде реалізовано, коли додамо EventService
            return new List<object>();
        }

        public async Task<object> GetUserTasksAsync()
        {
            try
            {
                var query = new GetUserTasksQuery(_userContext.CurrentUserId);
                var tasks = await _mediator.Send(query);
                return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get tasks error: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<object> GetUserExamsAsync()
        {
            try
            {
                var query = new GetUserExamsQuery(_userContext.CurrentUserId);
                var exams = await _mediator.Send(query);
                return exams;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get exams error: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<object> GetUserNotesAsync()
        {
            try
            {
                var query = new GetUserNotesQuery(_userContext.CurrentUserId);
                var notes = await _mediator.Send(query);
                return notes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get notes error: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<ServiceResult> ImportScheduleAsync()
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = "Імпорт розкладу буде реалізовано в майбутніх версіях"
            };
        }

        public async Task SaveUserSettingsAsync(object settings)
        {
            try
            {
                await _userService.SaveUserSettingsAsync(_userContext.CurrentUserId, settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save settings error: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ConnectTelegramAsync()
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = "Інтеграція з Telegram буде реалізована в майбутніх версіях"
            };
        }

        public async Task<ServiceResult> ConnectGoogleCalendarAsync()
        {
            return new ServiceResult
            {
                IsSuccess = false,
                ErrorMessage = "Інтеграція з Google Calendar буде реалізована в майбутніх версіях"
            };
        }

        public async Task DisconnectGoogleCalendarAsync()
        {
            await Task.CompletedTask;
        }
    }
}