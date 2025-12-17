namespace StudentHelper.WPF.UI
{
    /// <summary>
    /// Constants used throughout the application
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// Default Subject ID when no subject is selected
        /// </summary>
        public const int DefaultSubjectId = 1;

        /// <summary>
        /// Default color for new subjects (Blue)
        /// </summary>
        public const string DefaultSubjectColor = "#3357FF";

        /// <summary>
        /// Minimum password length
        /// </summary>
        public const int MinPasswordLength = 8;

        /// <summary>
        /// Days ahead to show "days remaining" for exams
        /// </summary>
        public const int DaysAheadForCountdown = 30;

        /// <summary>
        /// Days before exam to show urgent warning (red color)
        /// </summary>
        public const int UrgentExamDays = 7;
    }
}
