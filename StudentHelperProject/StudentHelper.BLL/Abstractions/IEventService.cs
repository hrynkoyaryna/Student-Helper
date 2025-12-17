using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Abstractions;

public interface IEventService
{
    Task<List<EventDto>> GetUserEventsAsync(int userId, CancellationToken ct = default);
    Task<List<EventDto>> GetEventsByDateRangeAsync(int userId, DateTime start, DateTime end, CancellationToken ct = default);
    Task<int> CreateAsync(EventDto dto, CancellationToken ct = default);
    Task UpdateAsync(EventDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
