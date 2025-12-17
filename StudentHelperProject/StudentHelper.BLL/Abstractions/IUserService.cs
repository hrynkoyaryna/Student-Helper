using StudentHelper.BLL.DTOs;
using System.Threading;

namespace StudentHelper.BLL.Abstractions
{
    public interface IUserService
    {
        System.Threading.Tasks.Task<UserDto?> AuthenticateAsync(
            string email,
            string password,
            CancellationToken ct = default);

        System.Threading.Tasks.Task<int> RegisterUserAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            CancellationToken ct = default);

        System.Threading.Tasks.Task<bool> SendPasswordResetCodeAsync(
            string email,
            CancellationToken ct = default);

        System.Threading.Tasks.Task<bool> VerifyPasswordResetCodeAsync(
            string email,
            string code,
            CancellationToken ct = default);

        System.Threading.Tasks.Task<bool> ResetPasswordAsync(
            string email,
            string newPassword,
            CancellationToken ct = default);

        System.Threading.Tasks.Task SaveUserSettingsAsync(
            int userId,
            object settings,
            CancellationToken ct = default);

        System.Threading.Tasks.Task<UserDto?> GetByEmailAsync(
            string email,
            CancellationToken ct = default);

        System.Threading.Tasks.Task<UserDto?> GetByIdAsync(
            int id,
            CancellationToken ct = default);

        System.Threading.Tasks.Task<bool> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword,
            CancellationToken ct = default);

        System.Threading.Tasks.Task<bool> UpdateUserProfileAsync(
            int userId,
            string firstName,
            string lastName,
            CancellationToken ct = default);
    }
}
