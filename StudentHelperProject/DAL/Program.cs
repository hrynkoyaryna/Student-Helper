// DAL/Program.cs
using DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🔍 Запуск діагностики БД та міграції...");

        try
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    string connectionString = context.Configuration.GetConnectionString("DefaultConnection")
                                            ?? throw new InvalidOperationException("Рядок підключення 'DefaultConnection' не знайдено.");

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(connectionString));
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Console.WriteLine("✅ Підключення до БД успішне. Застосовуємо міграції...");
            await context.Database.MigrateAsync();
            Console.WriteLine("✅ Міграції успішно застосовані (або база вже актуальна).");

            int usersCount = await context.Users.CountAsync();
            Console.WriteLine($"ℹ️ Користувачів в базі: {usersCount} користувач(ів)");
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "28P01")
        {
            Console.WriteLine($"❌ ПОМИЛКА АВТЕНТИФІКАЦІЇ (28P01): Невірний пароль/користувач.");
            Console.WriteLine("   Будь ласка, перевірте, чи правильно вказано ваш локальний пароль у файлі 'DAL/appsettings.json'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ КРИТИЧНА ПОМИЛКА: {ex.Message}");
            Console.WriteLine($"   Тип помилки: {ex.GetType().Name}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Внутрішня помилка: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
        Console.ReadKey();
    }
}