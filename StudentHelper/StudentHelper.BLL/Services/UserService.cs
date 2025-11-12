// StudentHelper.BLL/Services/UserService.cs
using StudentHelper.DAL.Repositories;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                return new UserDto(user.Id, user.FirstName, user.LastName, user.Email);
            }
            return null;
        }

        public async Task<bool> SendPasswordResetCodeAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null;
        }

        public async Task SaveUserSettingsAsync(int userId, object settings)
        {
            await Task.CompletedTask;
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? new UserDto(user.Id, user.FirstName, user.LastName, user.Email) : null;
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}