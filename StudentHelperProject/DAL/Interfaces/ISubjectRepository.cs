using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ISubjectRepository : IRepository<Subject>
    {
        Task<Subject?> GetByShortNameAsync(string shortName);
        Task<IEnumerable<Subject>> GetSubjectsByGroupAsync(int groupId);
    }
}