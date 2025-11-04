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
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Event>> GetUserEventsAsync(int userId)
        {
            return await _dbSet
                .Where(e => e.UserId == userId)
                .Include(e => e.Subject)
                .Include(e => e.Lecturer)
                .Include(e => e.Room)
                .OrderBy(e => e.StartAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(int userId, DateTime start, DateTime end)
        {
            return await _dbSet
                .Where(e => e.UserId == userId && 
                           e.StartAt >= start && 
                           e.EndAt <= end)
                .Include(e => e.Subject)
                .Include(e => e.Lecturer)
                .Include(e => e.Room)
                .OrderBy(e => e.StartAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(int userId, int daysAhead = 7)
        {
            var start = DateTime.UtcNow;
            var end = start.AddDays(daysAhead);
            
            return await _dbSet
                .Where(e => e.UserId == userId && 
                           e.StartAt >= start && 
                           e.StartAt <= end)
                .Include(e => e.Subject)
                .Include(e => e.Lecturer)
                .Include(e => e.Room)
                .OrderBy(e => e.StartAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsBySubjectAsync(int subjectId)
        {
            return await _dbSet
                .Where(e => e.SubjectId == subjectId)
                .Include(e => e.User)
                .Include(e => e.Lecturer)
                .Include(e => e.Room)
                .OrderBy(e => e.StartAt)
                .ToListAsync();
        }
    }
}