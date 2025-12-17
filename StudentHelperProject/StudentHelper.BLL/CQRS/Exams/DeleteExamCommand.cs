using MediatR;

namespace StudentHelper.BLL.CQRS.Exams;

public sealed record DeleteExamCommand(int Id) : IRequest<Unit>;
