using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Interfaces;
using DAL.Models;

namespace DAL.Repositories
{
    public class TaskRepository : BaseRepository<Models.Task>, ITaskRepository
    {
        public TaskRepository(AppDbContext context) : base(context) { }

        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetUserTasksAsync(int userId)
        {
            return await _context.Set<Models.Task>()
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        // 
        /*
        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetOverdueTasksAsync(int userId)
        {
            return await _context.Set<Models.Task>()
                .Where(t => t.UserId == userId && t.DueDate < DateTime.Now)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetTasksByDueDateAsync(int userId, DateTime dueDate)
        {
            return await _context.Set<Models.Task>()
                .Where(t => t.UserId == userId && t.DueDate.Date == dueDate.Date)
                .ToListAsync();
        }
        */

        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetTasksByPriorityAsync(int userId, string priority)
        {
            return await _context.Set<Models.Task>()
                .Where(t => t.UserId == userId && t.Priority == priority)
                .ToListAsync();
        }
    }
}
