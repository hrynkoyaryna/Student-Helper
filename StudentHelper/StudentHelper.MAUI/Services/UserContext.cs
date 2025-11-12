// StudentHelper.MAUI/Services/UserContext.cs
using Microsoft.Maui.Storage;

namespace StudentHelper.MAUI.Services;

public class UserContext : IUserContext
{
    private const string UserIdKey = "user_id";
    private const string IsAuthenticatedKey = "is_authenticated";

    public int CurrentUserId
    {
        get => Preferences.Get(UserIdKey, 0);
        private set => Preferences.Set(UserIdKey, value);
    }

    public bool IsAuthenticated
    {
        get => Preferences.Get(IsAuthenticatedKey, false);
        private set => Preferences.Set(IsAuthenticatedKey, value);
    }

    public void SetCurrentUser(int userId)
    {
        CurrentUserId = userId;
        IsAuthenticated = userId > 0;

        if (userId <= 0)
        {
            ClearUserData();
        }
    }

    public void ClearUserData()
    {
        Preferences.Remove(UserIdKey);
        Preferences.Remove(IsAuthenticatedKey);
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        if (IsAuthenticated && CurrentUserId > 0)
        {
            // Тут може бути перевірка токена з сервера
            return true;
        }
        return false;
    }
}