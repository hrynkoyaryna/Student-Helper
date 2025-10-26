using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHelper.BLL.Services
{
    using BLL.Abstractions;
    using BLL.DTOs;
    using DAL.Models;
    using DAL.Interfaces;

    public sealed class UserService(IUserRepository repo) : IUserService
    {
        public async Task<List<UserDto>> GetAllAsync(CancellationToken ct = default)
            => (await repo.GetAllAsync(ct))
               .Select(u => new UserDto(u.Id, u.Name, u.Email))
               .ToList();

        public async Task<UserDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var u = await repo.GetByIdAsync(id, ct);
            return u is null ? null : new UserDto(u.Id, u.Name, u.Email);
        }

        public async Task<int> CreateAsync(UserDto dto, CancellationToken ct = default)
        {
            var u = new User { Name = dto.Name, Email = dto.Email };
            await repo.AddAsync(u, ct);
            await repo.SaveChangesAsync(ct);
            return u.Id;
        }

        public async Task UpdateAsync(UserDto dto, CancellationToken ct = default)
        {
            var u = new User { Id = dto.Id, Name = dto.Name, Email = dto.Email };
            await repo.UpdateAsync(u, ct);
            await repo.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var u = await repo.GetByIdAsync(id, ct)
                    ?? throw new KeyNotFoundException($"User {id} not found");
            await repo.DeleteAsync(u, ct);
            await repo.SaveChangesAsync(ct);
        }
    }
}
