using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Subjects;

public sealed record GetSubjectByIdQuery(int Id) : IRequest<SubjectDto?>;
