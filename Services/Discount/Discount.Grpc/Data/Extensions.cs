using Discount.Grpc.Data;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public static class Extensions
    {
        public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DiscountContext>();
                context.Database.Migrate();
            }
            return app;
        }

    }
    
}