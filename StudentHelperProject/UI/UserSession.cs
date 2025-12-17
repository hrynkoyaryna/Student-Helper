namespace StudentHelper.WPF.UI
{
    public static class UserSession
    {
        public static int CurrentUserId { get; set; }
        public static string? CurrentUserEmail { get; set; }
        public static string? CurrentUserName { get; set; }
        public static string? CurrentUserFirstName { get; set; }
        public static string? CurrentUserLastName { get; set; }

        public static bool IsAuthenticated => CurrentUserId > 0;

        public static void Clear()
        {
            CurrentUserId = 0;
            CurrentUserEmail = null;
            CurrentUserName = null;
            CurrentUserFirstName = null;
            CurrentUserLastName = null;
        }
    }
}
