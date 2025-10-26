using BLL.Abstractions;
using BLL.CQRS.Queries;
using BLL.Services;
using DAL.Data;
using DAL.Interfaces;
using DAL.Repositories;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentHelper.BLL.CQRS.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        // EF Core (тимчасово — жорстко, потім винеси в appsettings.json)
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql("Host=localhost;Database=Student_Helper;Username=postgres;Password=Kvitochka06"));

        // DAL
        services.AddScoped<IUserRepository, UserRepository>();

        // BLL
        services.AddScoped<IUserService, UserService>();

        // MediatR
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(GetUsersQuery).Assembly));
    })
    .Build();
using var scope = host.Services.CreateScope();
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

var newId = await mediator.Send(new BLL.CQRS.Commands.CreateUserCommand("Kate", "k@k.com"));
var one = await mediator.Send(new BLL.CQRS.Queries.GetUserByIdQuery(newId));
Console.WriteLine(one is null ? "not found" : $"{one.Id} {one.Name} {one.Email}");


// Демо: запит і команда через MediatR
using var scope = host.Services.CreateScope();
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

var list = await mediator.Send(new GetUsersQuery());
Console.WriteLine($"Users via MediatR: {list.Count}");

await host.RunAsync();


class Program
{
    static readonly string dbPath = Path.Combine(AppContext.BaseDirectory, "student_helper.db");
    static readonly string connectionString = $"Data Source={dbPath};";
    static readonly Random Rng = new Random();

    static void Main()
    {
        CreateDatabaseSchema();

        using var conn = new SqliteConnection(connectionString);
        conn.Open();
        EnableFks(conn);

        int N = 30;
        SeedGroups(conn, N);
        SeedUsers(conn, N);
        SeedSubjects(conn, N);
        SeedTasks(conn, N);
        SeedEvents(conn, N);
        SeedExams(conn, N);

        // вивід
        PrintSummary(conn, new[]{
            "group_academic","user","subject","task","event","exam"
        });

        PrintTable(conn, "group_academic", "id, code, faculty, degree, year", 5);
        PrintTable(conn, "user", "id, first_name, last_name, email, status, group_id", 5);
        PrintTable(conn, "subject", "id, name, short_name, default_color", 5);
        PrintTable(conn, "task", "id, user_id, title, status, priority, due_at", 5);
        PrintTable(conn, "event", "id, user_id, title, type, start_at, end_at", 5);
        PrintTable(conn, "exam", "id, user_id, subject_id, title, exam_date, start_at", 5);

        Console.WriteLine("\nГотово. Enter для виходу...");
        Console.ReadLine();
    }
        static void CreateDatabaseSchema()
    {
        using var conn = new SqliteConnection(connectionString);
        conn.Open();
        EnableFks(conn);

        string sql = @"
CREATE TABLE IF NOT EXISTS group_academic (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  code TEXT NOT NULL,
  faculty TEXT NOT NULL,
  degree TEXT NOT NULL,
  year INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS user (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  first_name TEXT,
  last_name TEXT,
  email TEXT NOT NULL UNIQUE,
  password_hash TEXT,
  status TEXT NOT NULL DEFAULT 'active' CHECK (status IN ('active','inactive','blocked')),
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT,
  group_id INTEGER,
  FOREIGN KEY (group_id) REFERENCES group_academic(id) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS subject (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  name TEXT NOT NULL,
  short_name TEXT,
  description TEXT,
  default_color TEXT
);

CREATE TABLE IF NOT EXISTS event (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  user_id INTEGER NOT NULL,
  subject_id INTEGER,
  title TEXT NOT NULL,
  start_at TEXT NOT NULL,
  end_at TEXT NOT NULL,
  type TEXT NOT NULL CHECK (type IN ('personal','academic')),
  description TEXT,
  is_all_day INTEGER NOT NULL DEFAULT 0 CHECK (is_all_day IN (0,1)),
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT,
  FOREIGN KEY (user_id) REFERENCES user(id) ON DELETE CASCADE,
  FOREIGN KEY (subject_id) REFERENCES subject(id) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS task (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  user_id INTEGER NOT NULL,
  subject_id INTEGER,
  title TEXT NOT NULL,
  description TEXT,
  due_at TEXT,
  status TEXT NOT NULL DEFAULT 'current' CHECK (status IN ('current','completed','overdue')),
  priority TEXT NOT NULL DEFAULT 'medium' CHECK (priority IN ('low','medium','high','urgent')),
  created_at TEXT NOT NULL DEFAULT (datetime('now')),
  updated_at TEXT,
  completed_at TEXT,
  FOREIGN KEY (user_id) REFERENCES user(id) ON DELETE CASCADE,
  FOREIGN KEY (subject_id) REFERENCES subject(id) ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS exam (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  user_id INTEGER NOT NULL,
  subject_id INTEGER NOT NULL,
  title TEXT NOT NULL,
  exam_date TEXT NOT NULL,   -- YYYY-MM-DD
  start_at TEXT,             -- HH:mm:ss
  end_at TEXT,
  description TEXT,
  FOREIGN KEY (user_id) REFERENCES user(id) ON DELETE CASCADE,
  FOREIGN KEY (subject_id) REFERENCES subject(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_user_email         ON user(email);
CREATE INDEX IF NOT EXISTS IX_task_user_due      ON task(user_id, due_at);
CREATE INDEX IF NOT EXISTS IX_event_user_start   ON event(user_id, start_at);
CREATE INDEX IF NOT EXISTS IX_exam_user_date     ON exam(user_id, exam_date);
";
        using var cmd = new SqliteCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }

    static void EnableFks(SqliteConnection c)
    {
        using var pragma = new SqliteCommand("PRAGMA foreign_keys = ON;", c);
        pragma.ExecuteNonQuery();
    }

    static void SeedGroups(SqliteConnection c, int n)
    {
        if (Count(c, "group_academic") >= n) return;
        using var cmd = c.CreateCommand();
        cmd.CommandText = "INSERT INTO group_academic(code,faculty,degree,year) VALUES(@code,@fac,@deg,@year)";
        var facs = new[] { "FAMI", "CS", "Math", "Physics" };
        var degs = new[] { "BSc", "MSc" };
        for (int i = 0; i < n; i++)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@code", $"CS-{100 + i}");
            cmd.Parameters.AddWithValue("@fac", facs[i % facs.Length]);
            cmd.Parameters.AddWithValue("@deg", degs[i % degs.Length]);
            cmd.Parameters.AddWithValue("@year", 1 + (i % 4));
            cmd.ExecuteNonQuery();
        }
    }

    static void SeedUsers(SqliteConnection c, int n)
    {
        if (Count(c, "user") >= n) return;
        var groups = GetIds(c, "group_academic");
        var fns = new[] { "Ivan", "Sofiia", "Andrii", "Anastasiia", "Oksana", "Nazar", "Olena", "Dmytro" };
        var lns = new[] { "Kovalenko", "Shevchenko", "Melnyk", "Hrytsenko", "Luchyn", "Ziniak" };

        using var cmd = c.CreateCommand();
        cmd.CommandText = @"INSERT INTO user(first_name,last_name,email,password_hash,status,created_at,group_id)
                            VALUES(@fn,@ln,@em,@ph,@st,@ca,@gid)";
        for (int i = 0; i < n; i++)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@fn", fns[i % fns.Length]);
            cmd.Parameters.AddWithValue("@ln", lns[i % lns.Length]);
            cmd.Parameters.AddWithValue("@em", $"user{i + 1}@example.com");
            cmd.Parameters.AddWithValue("@ph", $"hash{i + 1:0000}");
            cmd.Parameters.AddWithValue("@st", (i % 10 == 0) ? "inactive" : "active");
            cmd.Parameters.AddWithValue("@ca", DateTime.UtcNow.ToString("o"));
            cmd.Parameters.AddWithValue("@gid", groups.Count > 0 ? groups[i % groups.Count] : (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }
    }

    static void SeedSubjects(SqliteConnection c, int n)
    {
        if (Count(c, "subject") >= n) return;
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"INSERT INTO subject(name,short_name,description,default_color)
                            VALUES(@n,@s,@d,@c)";
        string[] baseNames = { "Algorithms", "Databases", "Networks", "OOP", "AI", "Security", "Statistics", "UX" };
        string[] colors = { "#FF6B6B", "#4D96FF", "#6BCB77", "#FFD93D", "#B980F0" };
        for (int i = 0; i < n; i++)
        {
            string name = baseNames[i % baseNames.Length] + " " + (i + 1);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@s", baseNames[i % baseNames.Length]);
            cmd.Parameters.AddWithValue("@d", "Autogenerated subject");
            cmd.Parameters.AddWithValue("@c", colors[i % colors.Length]);
            cmd.ExecuteNonQuery();
        }
    }

    static void SeedTasks(SqliteConnection c, int n)
    {
        if (Count(c, "task") >= n) return;
        var users = GetIds(c, "user");
        var subjects = GetIds(c, "subject");
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"INSERT INTO task(user_id,subject_id,title,description,due_at,status,priority,created_at)
                            VALUES(@uid,@sid,@t,@d,@due,@st,@pr,@ca)";
        string[] statuses = { "current", "completed", "overdue" };
        string[] priorities = { "low", "medium", "high", "urgent" };
        for (int i = 0; i < n; i++)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@uid", users[i % users.Count]);
            cmd.Parameters.AddWithValue("@sid", subjects[i % subjects.Count]);
            cmd.Parameters.AddWithValue("@t", $"Task #{i + 1}");
            cmd.Parameters.AddWithValue("@d", "Autogenerated");
            cmd.Parameters.AddWithValue("@due", DateTime.UtcNow.AddDays(i % 20 - 5).ToString("o"));
            cmd.Parameters.AddWithValue("@st", statuses[i % statuses.Length]);
            cmd.Parameters.AddWithValue("@pr", priorities[i % priorities.Length]);
            cmd.Parameters.AddWithValue("@ca", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }
    }

    static void SeedEvents(SqliteConnection c, int n)
    {
        if (Count(c, "event") >= n) return;
        var users = GetIds(c, "user");
        var subjects = GetIds(c, "subject");
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"INSERT INTO event(user_id,subject_id,title,start_at,end_at,type,description,created_at)
                            VALUES(@uid,@sid,@t,@s,@e,@ty,@d,@c)";
        string[] types = { "personal", "academic" };
        for (int i = 0; i < n; i++)
        {
            var start = DateTime.UtcNow.AddDays(i % 12 - 3).AddHours(9 + (i % 5));
            var end = start.AddHours(1);

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@uid", users[i % users.Count]);
            cmd.Parameters.AddWithValue("@sid", subjects[i % subjects.Count]);
            cmd.Parameters.AddWithValue("@t", $"Event #{i + 1}");
            cmd.Parameters.AddWithValue("@s", start.ToString("o"));
            cmd.Parameters.AddWithValue("@e", end.ToString("o"));
            cmd.Parameters.AddWithValue("@ty", types[i % types.Length]);
            cmd.Parameters.AddWithValue("@d", "Autogenerated");
            cmd.Parameters.AddWithValue("@c", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }
    }

    static void SeedExams(SqliteConnection c, int n)
    {
        if (Count(c, "exam") >= n) return;
        var users = GetIds(c, "user");
        var subjects = GetIds(c, "subject");
        using var cmd = c.CreateCommand();
        cmd.CommandText = @"INSERT INTO exam(user_id,subject_id,title,exam_date,start_at,end_at,description)
                            VALUES(@uid,@sid,@t,@d,@s,@e,@desc)";
        for (int i = 0; i < n; i++)
        {
            var day = DateTime.UtcNow.AddDays(5 + (i % 25));
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@uid", users[i % users.Count]);
            cmd.Parameters.AddWithValue("@sid", subjects[i % subjects.Count]);
            cmd.Parameters.AddWithValue("@t", $"Exam #{i + 1}");
            cmd.Parameters.AddWithValue("@d", day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("@s", day.AddHours(9).ToString("HH:mm:ss"));
            cmd.Parameters.AddWithValue("@e", day.AddHours(11).ToString("HH:mm:ss"));
            cmd.Parameters.AddWithValue("@desc", "Autogenerated exam");
            cmd.ExecuteNonQuery();
        }
    }

    // ---------- Helpers ----------
    static int Count(SqliteConnection c, string table)
    {
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM {table}";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    static List<int> GetIds(SqliteConnection c, string table)
    {
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT id FROM {table}";
        var ids = new List<int>();
        using var r = cmd.ExecuteReader();
        while (r.Read()) ids.Add(Convert.ToInt32(r[0]));
        return ids;
    }

    static void PrintSummary(SqliteConnection c, string[] tables)
    {
        Console.WriteLine("\n=== ПІДСУМОК (рядків у таблицях) ===");
        foreach (var t in tables)
            Console.WriteLine($"{t,-16}: {Count(c, t)}");
    }

    static void PrintTable(SqliteConnection c, string table, string columns, int limit)
    {
        Console.WriteLine($"\n-- {table} --");
        using var cmd = c.CreateCommand();
        cmd.CommandText = $"SELECT {columns} FROM {table} LIMIT {limit}";
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            var parts = new List<string>();
            for (int i = 0; i < r.FieldCount; i++)
                parts.Add($"{r.GetName(i)}={r.GetValue(i)}");
            Console.WriteLine(string.Join(" | ", parts));
        }
    }
}
