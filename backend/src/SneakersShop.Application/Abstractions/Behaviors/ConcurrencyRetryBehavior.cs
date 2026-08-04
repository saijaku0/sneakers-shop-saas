using MediatR;

using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Exceptions;

namespace SneakersShop.Application.Abstractions.Behaviors;

public sealed class ConcurrencyRetryBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand
{
    private const int MaxRetries = 3;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch (ConcurrencyConflictException) when (attempt < MaxRetries)
            {
            }
        }

        throw new InvalidOperationException("Retry loop exited unexpectedly.");
    }
}