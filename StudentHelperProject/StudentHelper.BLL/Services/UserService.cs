using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;
using StudentHelper.BLL.CQRS.Users;
using DAL.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using UserEntity = DAL.Models.User;

namespace StudentHelper.BLL.Services
{
    /// <summary>
    /// Сервіс для управління користувачами.
    /// Надає функціонал для аутентифікації, реєстрації, управління профілем та відновлення пароля.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        
        // Локальний список користувачів для демонстраційних цілей
        private readonly List<UserEntity> _users = new();

        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="UserService"/>.
        /// </summary>
        /// <param name="unitOfWork">Одиниця роботи для доступу до репозиторіїв.</param>
        /// <param name="mediator">Медіатор для відправки CQRS-команд.</param>
        /// <param name="emailService">Сервіс для відправки електронних листів.</param>
        /// <param name="logger">Логер для запису подій.</param>
        public UserService(
            IUnitOfWork unitOfWork, 
            IMediator mediator, 
            IEmailService emailService, 
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Отримує всіх користувачів з локального списку.
        /// Примітка: Використовується лише для демонстрації.
        /// </summary>
        /// <returns>Колекція користувачів.</returns>
        public IEnumerable<UserEntity> GetAllUsers()
        {
            return _users;
        }

        /// <summary>
        /// Додає користувача до локального списку.
        /// Примітка: Використовується лише для демонстрації.
        /// </summary>
        /// <param name="user">Об'єкт користувача для додавання.</param>
        public void AddUser(UserEntity user)
        {
            if (user.Id == 0)
                user.Id = _users.Count + 1;

            _users.Add(user);
        }

        /// <summary>
        /// Видаляє користувача з локального списку за ідентифікатором.
        /// Примітка: Використовується лише для демонстрації.
        /// </summary>
        /// <param name="id">Ідентифікатор користувача.</param>
        public void DeleteUser(int id)
        {
            var user = _users.Find(x => x.Id == id);
            if (user != null)
                _users.Remove(user);
        }

        /// <summary>
        /// Аутентифікує користувача за електронною поштою та паролем.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="password">Пароль користувача.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>Об'єкт користувача у випадку успішної аутентифікації; інакше - null.</returns>
        public async Task<UserDto?> AuthenticateAsync(
            string email,
            string password,
            CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);

            if (user == null)
                return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordValid)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GroupId = user.GroupId
            };
        }

        /// <summary>
        /// Реєструє нового користувача в системі.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="password">Пароль користувача.</param>
        /// <param name="firstName">Ім'я користувача.</param>
        /// <param name="lastName">Прізвище користувача.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>Ідентифікатор створеного користувача.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Викидається, якщо користувач з вказаною електронною поштою вже існує.
        /// </exception>
        public async Task<int> RegisterUserAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            CancellationToken ct = default)
        {
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(email);
            if (existingUser != null)
                throw new System.InvalidOperationException("Користувач з цією електронною поштою вже існує.");

            var command = new CreateUserCommand(firstName, lastName, email, password);
            var userId = await _mediator.Send(command, ct);

            return userId;
        }

        /// <summary>
        /// Надсилає код для скидання пароля на вказану електронну пошту.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>
        /// true - код успішно згенеровано та збережено (лист може бути не відправлено);
        /// false - користувача не знайдено.
        /// </returns>
        public async Task<bool> SendPasswordResetCodeAsync(
            string email,
            CancellationToken ct = default)
        {
            _logger.LogInformation($"Виклик SendPasswordResetCodeAsync для email: {email}");

            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning($"Користувача не знайдено для email: {email}");
                return false;
            }

            // Генерація випадкового 6-значного коду
            string code = new Random().Next(100000, 999999).ToString();
            _logger.LogInformation($"Згенеровано код скидання: {code} для користувача: {email}");

            // Встановлення коду та терміну дії (10 хвилин)
            user.PasswordResetCode = code;
            user.PasswordResetCodeExpires = DateTime.UtcNow.AddMinutes(10);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Код скидання збережено в базу даних для користувача: {email}");

            // Відправка коду по електронній пошті
            string userName = $"{user.FirstName} {user.LastName}".Trim();
            _logger.LogInformation($"Спроба відправки листа з кодом скидання на {email}");
            bool emailSent = await _emailService.SendPasswordResetCodeAsync(email, code, userName);

            if (emailSent)
            {
                _logger.LogInformation($"Лист з кодом скидання успішно відправлено на {email}");
            }
            else
            {
                _logger.LogError($"Не вдалося відправити лист з кодом скидання на {email}. Код для тестування: {code}");
                // Навіть якщо лист не відправлено, код зберігається в БД, користувач може його використати
                System.Diagnostics.Debug.WriteLine($"[ТЕСТУВАННЯ] Код скидання для {email}: {code}");
            }

            // Повертаємо true навіть якщо лист не відправлено - код збережено в базі даних
            return true;
        }

        /// <summary>
        /// Зберігає налаштування користувача.
        /// Примітка: Реалізація відсутня, метод залишений як заглушка.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="settings">Налаштування користувача.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>Завершене завдання.</returns>
        public Task SaveUserSettingsAsync(
            int userId,
            object settings,
            CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Отримує користувача за електронною поштою.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>Об'єкт користувача або null, якщо користувача не знайдено.</returns>
        public async Task<UserDto?> GetByEmailAsync(
            string email,
            CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GroupId = user.GroupId
            };
        }

        /// <summary>
        /// Отримує користувача за ідентифікатором.
        /// </summary>
        /// <param name="id">Ідентифікатор користувача.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>Об'єкт користувача або null, якщо користувача не знайдено.</returns>
        public async Task<UserDto?> GetByIdAsync(
            int id,
            CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GroupId = user.GroupId
            };
        }

        /// <summary>
        /// Змінює пароль користувача.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="currentPassword">Поточний пароль користувача.</param>
        /// <param name="newPassword">Новий пароль користувача.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>
        /// true - пароль успішно змінено;
        /// false - користувача не знайдено або поточний пароль невірний.
        /// </returns>
        public async Task<bool> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword,
            CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return false;

            // Перевірка поточного пароля
            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);

            if (!isCurrentPasswordValid)
                return false;

            // Хешування нового пароля
            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Оновлення пароля
            user.PasswordHash = newPasswordHash;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Оновлює профіль користувача.
        /// </summary>
        /// <param name="userId">Ідентифікатор користувача.</param>
        /// <param name="firstName">Нове ім'я користувача.</param>
        /// <param name="lastName">Нове прізвище користувача.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>
        /// true - профіль успішно оновлено;
        /// false - користувача не знайдено.
        /// </returns>
        public async Task<bool> UpdateUserProfileAsync(
            int userId,
            string firstName,
            string lastName,
            CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
                return false;

            user.FirstName = firstName;
            user.LastName = lastName;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Перевіряє код для скидання пароля.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="code">Код для перевірки.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>
        /// true - код валідний та не протермінований;
        /// false - код невірний, протермінований або користувача не знайдено.
        /// </returns>
        public async Task<bool> VerifyPasswordResetCodeAsync(
            string email,
            string code,
            CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                return false;

            // Перевірка відповідності коду та терміну дії
            if (user.PasswordResetCode != code)
                return false;

            if (user.PasswordResetCodeExpires == null || DateTime.UtcNow > user.PasswordResetCodeExpires)
                return false;

            return true;
        }

        /// <summary>
        /// Скидає пароль користувача після успішної перевірки коду.
        /// </summary>
        /// <param name="email">Електронна пошта користувача.</param>
        /// <param name="newPassword">Новий пароль.</param>
        /// <param name="ct">Токен скасування операції.</param>
        /// <returns>
        /// true - пароль успішно скинуто;
        /// false - користувача не знайдено.
        /// </returns>
        public async Task<bool> ResetPasswordAsync(
            string email,
            string newPassword,
            CancellationToken ct = default)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                return false;

            // Хешування нового пароля
            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Оновлення пароля та очищення коду скидання
            user.PasswordHash = newPasswordHash;
            user.PasswordResetCode = null;
            user.PasswordResetCodeExpires = null;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}