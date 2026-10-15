using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Data.Interceptors;
using OrderingApplication.Data;
using OrderingInfastructure.Data;
using OrderingInfastructure.Data.Interspector;

namespace OrderingInfastructure
{
    public static class DependanceInjection
    {
        public static IServiceCollection AddOrderingInfastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<AuditTableEntityInterpector>();
            services.AddScoped<DispatchDomainEventsInterceptor>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);

                options.AddInterceptors(
                    sp.GetRequiredService<AuditTableEntityInterpector>(),
                    sp.GetRequiredService<DispatchDomainEventsInterceptor>());
            });
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            return services;
        }
    }
}