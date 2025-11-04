namespace StudentHelper.MAUI.Services
{
    public class AppService : IAppService
    {
        public Task<ServiceResult> AuthenticateUserAsync(string email, string password)
        {

            return Task.FromResult(new ServiceResult { IsSuccess = true });
        }

        public Task<ServiceResult> AuthenticateWithOpenIdAsync()
        {
            return Task.FromResult(new ServiceResult { IsSuccess = true });
        }

        public Task<ServiceResult> RegisterUserAsync(string firstName, string lastName, string email, string password)
        {
            return Task.FromResult(new ServiceResult { IsSuccess = true });
        }

        public Task<ServiceResult> SendPasswordResetCodeAsync(string email)
        {
            return Task.FromResult(new ServiceResult { IsSuccess = true });
        }

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }

        public Task<object> GetUserEventsAsync()
        {
            return Task.FromResult<object>(new object());
        }

        public Task<object> GetUserTasksAsync()
        {
            return Task.FromResult<object>(new object());
        }

        public Task<object> GetUserExamsAsync()
        {
            return Task.FromResult<object>(new object());
        }

        public Task<object> GetUserNotesAsync()
        {
            return Task.FromResult<object>(new object());
        }

        public Task<ServiceResult> ImportScheduleAsync()
        {
            return Task.FromResult(new ServiceResult { IsSuccess = true });
        }

        public Task SaveUserSettingsAsync(object settings)
        {
            return Task.CompletedTask;
        }

        public Task<ServiceResult> ConnectTelegramAsync()
        {
            return Task.FromResult(new ServiceResult { IsSuccess = true });
        }

        public Task<ServiceResult> ConnectGoogleCalendarAsync()
        {
            return Task.FromResult(new ServiceResult { IsSuccess = true });
        }

        public Task DisconnectGoogleCalendarAsync()
        {
            return Task.CompletedTask;
        }
    }
}