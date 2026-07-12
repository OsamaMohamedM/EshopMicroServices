namespace Ordering.Api
{
    public static class DependanceInjection
    {
        public static IServiceCollection AddOrderingApiServices(this IServiceCollection services)
        {
            return services;
        }

        public static WebApplication AddOrderingApiUses(this WebApplication services)
        {
            return services;
        }
    }
}