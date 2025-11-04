using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface INoteRepository : IRepository<Note>
    {
        Task<IEnumerable<Note>> GetUserNotesAsync(int userId);
        Task<IEnumerable<Note>> GetPinnedNotesAsync(int userId);
    }
}