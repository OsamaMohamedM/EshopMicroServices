using BuildingBlocks.Exceptions.Handler;
using Carter;

namespace Ordering.Api
{
    public static class DependanceInjection
    {
        public static IServiceCollection AddOrderingApiServices(this IServiceCollection services)
        {
            services.AddCarter();
            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddProblemDetails();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName);
            });

            return services;
        }

        public static WebApplication UseApiServices(this WebApplication services)
        {
            services.MapCarter();
            services.UseExceptionHandler();
            services.UseSwagger();
            services.UseSwaggerUI();

            return services;
        }

        public static WebApplication AddOrderingApiUses(this WebApplication services)
        {
            return services.UseApiServices();
        }
    }
}
