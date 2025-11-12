using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Abstractions;

public interface INoteService
{
    Task<List<NoteDto>> GetUserNotesAsync(int userId, CancellationToken ct = default);
    Task<List<NoteDto>> GetPinnedNotesAsync(int userId, CancellationToken ct = default);

    Task<int> CreateAsync(NoteDto dto, CancellationToken ct = default);
    Task UpdateAsync(NoteDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
