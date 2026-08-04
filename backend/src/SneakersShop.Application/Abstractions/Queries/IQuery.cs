using MediatR;

using SneakersShop.Domain.Common.Results;

namespace SneakersShop.Application.Abstractions.Queries;

public interface IQuery<T> : IRequest<Result<T>>;