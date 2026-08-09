using BuildingBlocks.Behaviors.LoggingBehavior;
using BuildingBlocks.Behaviors.ValidationBehavior;
using BuildingBlocksMessageing.MassTransit;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using System.Reflection;

namespace OrderingApplication
{
    public static class DependanceInjection
    {
        public static IServiceCollection AddOrderingApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(LoggingBehaviorr<,>));
                config.AddOpenBehavior(typeof(ValidatorBehavior<,>));
            });
            services.AddFeatureManagement();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());
            return services;
        }
    }
}