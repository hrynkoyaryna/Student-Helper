using MediatR;

namespace StudentHelper.BLL.CQRS.Notes;

public sealed record UpdateNoteCommand(
    int Id,
    int UserId,
    string Title,
    string Content,
    bool IsPinned
) : IRequest;
