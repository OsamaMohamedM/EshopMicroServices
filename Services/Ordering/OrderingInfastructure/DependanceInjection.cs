using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderingApplication.Data;
using OrderingInfastructure.Data;

namespace OrderingInfastructure
{
    public static class DependanceInjection
    {
        public static IServiceCollection AddOrderingInfastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(new Data.Interspector.AuditTableEntityInterpector());
            });
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            return services;
        }
    }
}