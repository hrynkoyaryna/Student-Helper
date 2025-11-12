using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Subjects;

public sealed record GetAllSubjectsQuery() : IRequest<List<SubjectDto>>;
