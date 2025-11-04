using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByGroupAsync(int groupId);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserWithDetailsAsync(int id);
    }
}