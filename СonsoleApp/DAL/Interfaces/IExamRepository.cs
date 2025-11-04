using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IExamRepository : IRepository<Exam>
    {
        Task<IEnumerable<Exam>> GetUserExamsAsync(int userId);
        Task<IEnumerable<Exam>> GetUpcomingExamsAsync(int userId, int daysAhead = 30);
        Task<IEnumerable<Exam>> GetExamsBySubjectAsync(int subjectId);
        Task<IEnumerable<Exam>> GetExamsByDateRangeAsync(int userId, DateTime start, DateTime end);
    }
}