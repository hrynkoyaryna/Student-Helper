using DAL.Interfaces;
using DAL.Models;
using StudentHelper.BLL.Abstractions;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.Services;

public sealed class ExamService : IExamService
{
    private readonly IExamRepository _repo;

    public ExamService(IExamRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ExamDto>> GetUserExamsAsync(int userId, CancellationToken ct = default)
    {
        var exams = await _repo.GetUserExamsAsync(userId);
        return exams.Select(MapToDto).ToList();
    }

    public async Task<List<ExamDto>> GetUpcomingExamsAsync(int userId, int daysAhead = 30, CancellationToken ct = default)
    {
        var exams = await _repo.GetUpcomingExamsAsync(userId, daysAhead);
        return exams.Select(MapToDto).ToList();
    }

    public async Task<int> CreateAsync(ExamDto dto, CancellationToken ct = default)
    {
        var e = new Exam
        {
            UserId = dto.UserId,
            SubjectId = dto.SubjectId,
            Title = dto.Title,
            ExamDate = dto.ExamDate,
            StartAt = dto.StartTime,
            EndAt = dto.EndTime,
            Description = dto.Description
        };

        await _repo.AddAsync(e, ct);
        await _repo.SaveChangesAsync(ct);
        return e.Id;
    }

    public async Task UpdateAsync(ExamDto dto, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(dto.Id, ct)
                ?? throw new KeyNotFoundException($"Exam {dto.Id} not found");

        e.UserId = dto.UserId;
        e.SubjectId = dto.SubjectId;
        e.Title = dto.Title;
        e.ExamDate = dto.ExamDate;
        e.StartAt = dto.StartTime;
        e.EndAt = dto.EndTime;
        e.Description = dto.Description;

        _repo.Update(e);
        await _repo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Exam {id} not found");

        _repo.Remove(e);
        await _repo.SaveChangesAsync(ct);
    }

    private static ExamDto MapToDto(Exam e) =>
        new(
            e.Id,
            e.UserId,
            e.SubjectId,
            e.Title,
            e.ExamDate,
            e.StartAt,
            e.EndAt,
            e.Description
        );
}
