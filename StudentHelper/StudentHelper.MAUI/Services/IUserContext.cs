// StudentHelper.MAUI/Services/IUserContext.cs
namespace StudentHelper.MAUI.Services;

public interface IUserContext
{
    int CurrentUserId { get; }
    bool IsAuthenticated { get; }
    void SetCurrentUser(int userId);
}