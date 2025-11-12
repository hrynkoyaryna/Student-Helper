using MediatR;

namespace StudentHelper.BLL.CQRS.Notes;

public sealed record DeleteNoteCommand(int Id) : IRequest;
