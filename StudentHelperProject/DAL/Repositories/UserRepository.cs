using Microsoft.EntityFrameworkCore;
using DAL.Interfaces;
using DAL.Models;
using DAL.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.Group)
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByGroupAsync(int groupId)
        {
            return await _dbSet
                .Where(u => u.GroupId == groupId)
                .Include(u => u.Group)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Group)
                .Include(u => u.Profile)
                .Include(u => u.NotificationSetting)
                .Include(u => u.AuthIdentities)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}