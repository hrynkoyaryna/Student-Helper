using MediatR;
using StudentHelper.BLL.DTOs;

namespace StudentHelper.BLL.CQRS.Notes;

public sealed record GetUserNotesQuery(int UserId) : IRequest<List<NoteDto>>;
