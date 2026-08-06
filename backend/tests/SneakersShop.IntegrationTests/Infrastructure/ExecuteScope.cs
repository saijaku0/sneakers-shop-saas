using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace SneakersShop.IntegrationTests.Infrastructure;

public class ExecuteScope
{
    private IServiceProvider? _serviceProvider;

    public void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider is not initialized. Call Initialize() first.");

        using var scope = _serviceProvider.CreateScope();
        await action(scope.ServiceProvider);
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider is not initialized. Call Initialize() first.");

        using var scope = _serviceProvider.CreateScope();
        return await action(scope.ServiceProvider);
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider is not initialized. Call Initialize() first.");

        using var scope = _serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(request);
    }
}