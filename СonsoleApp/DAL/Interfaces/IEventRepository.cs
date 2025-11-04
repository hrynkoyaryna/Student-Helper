using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<IEnumerable<Event>> GetUserEventsAsync(int userId);
        Task<IEnumerable<Event>> GetEventsByDateRangeAsync(int userId, DateTime start, DateTime end);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync(int userId, int daysAhead = 7);
        Task<IEnumerable<Event>> GetEventsBySubjectAsync(int subjectId);
    }
}