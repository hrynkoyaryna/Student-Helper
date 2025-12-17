using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Exams;

public sealed record GetUserExamsQuery(int UserId) : IRequest<List<ExamDto>>;
