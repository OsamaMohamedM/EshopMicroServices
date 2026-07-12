using BuildingBlocks.Behaviors.LoggingBehavior;
using BuildingBlocks.Behaviors.ValidationBehavior;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OrderingApplication
{
    public static class DependanceInjection
    {
        public static IServiceCollection AddOrderingApplication(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(LoggingBehaviorr<,>));
                config.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}