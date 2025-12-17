using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace StudentHelper.WPF.UI
{
    /// <summary>
    /// Configures Serilog logging for the application
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        /// Initializes Serilog with file and console sinks
        /// </summary>
        public static void ConfigureLogging()
        {
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "StudentHelper",
                "Logs",
                "log-.txt"
            );

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 10_000_000
                )
                .CreateLogger();

            Log.Information("========================================");
            Log.Information("StudentHelper Application Started");
            Log.Information("========================================");
        }

        /// <summary>
        /// Closes and flushes the log
        /// </summary>
        public static void CloseLogging()
        {
            Log.Information("========================================");
            Log.Information("StudentHelper Application Stopped");
            Log.Information("========================================");
            Log.CloseAndFlush();
        }
    }
}
