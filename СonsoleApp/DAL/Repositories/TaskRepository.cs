using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Interfaces;
using DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; 

namespace DAL.Repositories
{
    public class TaskRepository : BaseRepository<Models.Task>, ITaskRepository  
    {
        public TaskRepository(AppDbContext context) : base(context) { }

        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetUserTasksAsync(int userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId)
                .Include(t => t.Subject)
                .OrderBy(t => t.DueDate) 
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetTasksByPriorityAsync(int userId, string priority)
        {
            return await _dbSet
                .Where(t => t.UserId == userId && t.Priority == priority)
                .Include(t => t.Subject)
                .OrderBy(t => t.DueDate) 
                .ToListAsync();
        }
        
        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetOverdueTasksAsync(int userId)
        {
            var now = DateTime.UtcNow;

            return await _dbSet
                .Where(t => t.UserId == userId && 
                           t.DueDate < now && 
                           t.Status != "completed")
                .Include(t => t.Subject)
                .OrderBy(t => t.DueDate) 
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetTasksByDueDateAsync(int userId, DateTime dueDate)
        {
            var startOfDay = dueDate.Date;
            var endOfDay = dueDate.Date.AddDays(1).AddTicks(-1);
            
            return await _dbSet
                .Where(t => t.UserId == userId && 
                           t.DueDate >= startOfDay && 
                           t.DueDate <= endOfDay) 
                .Include(t => t.Subject)
                .OrderBy(t => t.DueDate) 
                .ToListAsync();
        }
    }
}