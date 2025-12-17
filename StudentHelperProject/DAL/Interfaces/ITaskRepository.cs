using System.Linq.Expressions;
using DAL.Models;

namespace DAL.Interfaces
{
    public interface ITaskRepository : IRepository<Models.Task>  
    {
        System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetUserTasksAsync(int userId);  
        //System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetOverdueTasksAsync(int userId);
        //System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetTasksByDueDateAsync(int userId, DateTime dueDate);
        System.Threading.Tasks.Task<IEnumerable<Models.Task>> GetTasksByPriorityAsync(int userId, string priority);
    }
}