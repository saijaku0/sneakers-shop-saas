using MediatR;

using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Abstractions.Commands;

public interface ICommand : IRequest<Result>;
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;