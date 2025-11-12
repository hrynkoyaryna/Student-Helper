// StudentHelper.BLL/Abstractions/IUserService.cs
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services
{
    public interface IUserService
    {
        Task<UserDto?> AuthenticateAsync(string email, string password);
        Task<bool> SendPasswordResetCodeAsync(string email);
        Task SaveUserSettingsAsync(int userId, object settings);
        Task<UserDto?> GetByIdAsync(int id);
    }
}