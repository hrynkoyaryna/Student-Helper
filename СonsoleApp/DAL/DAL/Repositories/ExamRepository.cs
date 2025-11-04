using Microsoft.EntityFrameworkCore;
using DAL.Interfaces;
using DAL.Models;
using DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ExamRepository : BaseRepository<Exam>, IExamRepository
    {
        public ExamRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Exam>> GetUserExamsAsync(int userId)
        {
            return await _dbSet
                .Where(e => e.UserId == userId)
                .Include(e => e.Subject)
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.StartAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetUpcomingExamsAsync(int userId, int daysAhead = 30)
        {
            var start = DateTime.UtcNow;
            var end = start.AddDays(daysAhead);
            
            return await _dbSet
                .Where(e => e.UserId == userId && 
                           e.ExamDate >= start.Date && 
                           e.ExamDate <= end.Date)
                .Include(e => e.Subject)
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.StartAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetExamsBySubjectAsync(int subjectId)
        {
            return await _dbSet
                .Where(e => e.SubjectId == subjectId)
                .Include(e => e.User)
                .OrderBy(e => e.ExamDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetExamsByDateRangeAsync(int userId, DateTime start, DateTime end)
        {
            return await _dbSet
                .Where(e => e.UserId == userId && 
                           e.ExamDate >= start.Date && 
                           e.ExamDate <= end.Date)
                .Include(e => e.Subject)
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.StartAt)
                .ToListAsync();
        }
    }
}