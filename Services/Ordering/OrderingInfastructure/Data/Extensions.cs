using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OrderingInfastructure.Data
{
    public static class Extensions
    {
        public static async Task MigrateAndSeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var services = serviceScope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetService<ILoggerFactory>()?.CreateLogger("DatabaseMigration");

            const int maxRetries = 6;
            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    logger?.LogInformation("Attempting to apply database migrations and seed data (Attempt {Attempt}/{MaxRetries})...", attempt, maxRetries);
                    await context.Database.MigrateAsync();
                    await DatabaseInitialiser.SeedAsync(context);
                    logger?.LogInformation("Database migration and seeding completed successfully.");
                    return;
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    var delaySeconds = Math.Min((int)Math.Pow(2, attempt), 30);
                    logger?.LogWarning(ex, "Database migration failed on attempt {Attempt}/{MaxRetries}. Retrying in {Delay} seconds...", attempt, maxRetries, delaySeconds);
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }
        }
    }
}
