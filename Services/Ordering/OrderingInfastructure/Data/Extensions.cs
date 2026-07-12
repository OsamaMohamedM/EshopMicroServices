using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OrderingInfastructure.Data
{
    public static class Extensions
    {
        public static async Task MigrateAndSeedDatabaseAsync(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await context.Database.MigrateAsync();
                await DatabaseInitialiser.SeedAsync(context);
            }
        }
    }
}
