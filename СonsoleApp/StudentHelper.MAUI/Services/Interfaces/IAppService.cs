namespace StudentHelper.MAUI.Services
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public interface IAppService
    {
        // Authentication
        Task<ServiceResult> AuthenticateUserAsync(string email, string password);
        Task<ServiceResult> AuthenticateWithOpenIdAsync();
        Task<ServiceResult> RegisterUserAsync(string firstName, string lastName, string email, string password);
        Task<ServiceResult> SendPasswordResetCodeAsync(string email);
        Task LogoutAsync();

        // Data operations
        Task<object> GetUserEventsAsync();
        Task<object> GetUserTasksAsync();
        Task<object> GetUserExamsAsync();
        Task<object> GetUserNotesAsync();

        // Schedule
        Task<ServiceResult> ImportScheduleAsync();

        // Settings
        Task SaveUserSettingsAsync(object settings);
        Task<ServiceResult> ConnectTelegramAsync();
        Task<ServiceResult> ConnectGoogleCalendarAsync();
        Task DisconnectGoogleCalendarAsync();
    }
}