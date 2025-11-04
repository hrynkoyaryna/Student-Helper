using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IGroupAcademicRepository : IRepository<GroupAcademic>
    {
        Task<GroupAcademic?> GetByCodeAsync(string code);
        Task<IEnumerable<GroupAcademic>> GetGroupsByFacultyAsync(string faculty);
        Task<IEnumerable<GroupAcademic>> GetGroupsByYearAsync(int year);
    }
}