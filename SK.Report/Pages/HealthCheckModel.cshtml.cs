using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using SK.Report.Data;

namespace SK.Report.Pages
{
    public class HealthCheckModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public string HealthStatusJson { get; private set; } = string.Empty;

        public HealthCheckModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var status = new
            {
                Application = "Healthy",
                PostgreSql = await CheckPostgresAsync(),
                Sqlite = await CheckSqliteAsync()
            };

            HealthStatusJson = JsonSerializer.Serialize(status);
        }

        private async Task<string> CheckPostgresAsync() =>
            await CheckDatabaseAsync<ReportContext>(_configuration.GetConnectionString("SKConnectionString"), DatabaseType.PostgreSql);

        private async Task<string> CheckSqliteAsync() =>
            await CheckDatabaseAsync<AppDbContext>(_configuration.GetConnectionString("SqliteConnectionString"), DatabaseType.Sqlite);

        private static async Task<string> CheckDatabaseAsync<TContext>(string connectionString, DatabaseType databaseType) where TContext : DbContext
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<TContext>();
                switch (databaseType)
                {
                    case DatabaseType.PostgreSql:
                        optionsBuilder.UseNpgsql(connectionString);
                        break;
                    case DatabaseType.Sqlite:
                        optionsBuilder.UseSqlite(connectionString);
                        break;
                }
                using var db = (TContext)Activator.CreateInstance(typeof(TContext), optionsBuilder.Options)!;
                await db.Database.OpenConnectionAsync();
                await db.Database.CloseConnectionAsync();
                return "Healthy";
            }
            catch (Exception ex)
            {
                return $"Unhealthy: {ex.Message}";
            }
        }
    }
}

public enum DatabaseType
{
    PostgreSql,
    Sqlite
}