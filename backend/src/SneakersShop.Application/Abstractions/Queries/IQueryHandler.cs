using MediatR;

using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Abstractions.Queries;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}