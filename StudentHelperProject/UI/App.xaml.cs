using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using StudentHelper.BLL;
using DAL;
using StudentHelper.WPF.UI.ViewModels;
using StudentHelper.WPF.UI.Views;

namespace StudentHelper.WPF.UI
{
    public partial class App : Application
    {
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize Serilog
            LoggingConfiguration.ConfigureLogging();

            try
            {
                Log.Information("Initializing application...");

                var builder = Host.CreateDefaultBuilder();

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    var basePath = AppDomain.CurrentDomain.BaseDirectory;
                    config.SetBasePath(basePath);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                });

                builder.ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                    Log.Information("Configuring services...");

                    services.AddDataLayer(connectionString);
                    services.AddBusinessLogic(context.Configuration);

                    // Add Serilog
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddSerilog(dispose: true);
                    });

                    services.AddScoped<AuthViewModel>();
                    services.AddScoped<RegisterViewModel>();
                    services.AddScoped<MainViewModel>();
                    services.AddScoped<CalendarViewModel>();
                    services.AddScoped<TasksViewModel>();
                    services.AddScoped<ExamsViewModel>();
                    services.AddScoped<NotesViewModel>();

                    services.AddSingleton<AuthWindow>();

                    Log.Information("Services configured successfully");
                });

                _host = builder.Build();

                ServiceLocator.Initialize(_host.Services);

                Log.Information("Opening authentication window...");

                var authWindow = _host.Services.GetRequiredService<AuthWindow>();
                authWindow.Show();

                Log.Information("Application started successfully");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application startup failed");
                MessageBox.Show($"Fatal error during startup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Application shutting down...");
            _host?.Dispose();
            LoggingConfiguration.CloseLogging();
            base.OnExit(e);
        }
    }
}
