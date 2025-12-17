using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Task = System.Threading.Tasks.Task;
public sealed class DbLogWriter
{
    private readonly AppDbContext _db;

    public DbLogWriter(AppDbContext db) => _db = db;

    public async Task WriteAsync(LogEventLevel level, string message, string logger, string? exception = null, string? contextJson = null)
    {
        _db.AppLogs.Add(new AppLog
        {
            Level = level.ToString(),
            Message = message,
            Logger = logger ?? string.Empty,
            Exception = exception ?? string.Empty,
            Context = contextJson ?? "{}",
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}
