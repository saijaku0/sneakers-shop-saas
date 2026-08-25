using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Common.Behaviors;

namespace SneakersShop.Application;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ConcurrencyRetryBehavior<,>));

        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(ConcurrencyRetryBehavior<,>));
        });

        return services;
    }
}