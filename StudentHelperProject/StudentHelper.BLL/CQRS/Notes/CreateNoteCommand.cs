using MediatR;

namespace StudentHelper.BLL.CQRS.Notes;

public sealed record CreateNoteCommand(
    int UserId,
    string Title,
    string Content,
    bool IsPinned
) : IRequest<int>;
