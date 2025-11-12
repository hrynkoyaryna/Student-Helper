using DAL.Data;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🔍 Перевірка підключення до БД...");

        try
        {
            // Створюємо контекст БД
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Database=Student_Helper;Username=postgres;Password=Kvitochka06");
            
            using var context = new AppDbContext(optionsBuilder.Options);

            // Перевіряємо підключення
            bool canConnect = await context.Database.CanConnectAsync();
            
            if (canConnect)
            {
                Console.WriteLine("✅ Підключення до БД успішне!");
                
                // Перевіряємо чи є користувачі
                int usersCount = await context.Users.CountAsync();
                
                if (usersCount > 0)
                {
                    Console.WriteLine($"✅ Користувачі є в базі: {usersCount} користувач(ів)");
                }
                else
                {
                    Console.WriteLine("ℹ️ Користувачів в базі немає");
                }
            }
            else
            {
                Console.WriteLine("❌ Не вдалося підключитися до БД");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Сталася помилка: {ex.Message}");
        }

        Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
        Console.ReadKey();
    }
}