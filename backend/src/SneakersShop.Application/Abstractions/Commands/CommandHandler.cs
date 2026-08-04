using MediatR;

using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Abstractions.Commands;

public abstract class CommandHandler<TCommand>(IUnitOfWork unitOfWork) : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
    protected readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
    {
        Result result = await HandleCommandAsync(command, cancellationToken);
        if (result.IsSuccess)
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }

    protected abstract Task<Result> HandleCommandAsync(TCommand command, CancellationToken cancellationToken);
}

public abstract class CommandHandler<TCommand, TResponse>(IUnitOfWork unitOfWork) : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
    protected readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<TResponse>> Handle(TCommand request, CancellationToken cancellationToken)
    {
        Result<TResponse> result = await HandleCommandAsync(request, cancellationToken);
        if (result.IsSuccess)
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
    protected abstract Task<Result<TResponse>> HandleCommandAsync(TCommand request, CancellationToken cancellationToken);
}