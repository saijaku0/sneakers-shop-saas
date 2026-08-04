using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Warehouse.Errors;

namespace SneakersShop.Application.Warehouse.Commands.ReserveStock;

internal sealed class ReserveStockCommandHandler(
    IWarehouseItemRepository repository,
    IUnitOfWork unitOfWork)
    : CommandHandler<ReserveStockCommand>(unitOfWork)
{
    protected override async Task<Result> HandleCommandAsync(
        ReserveStockCommand request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.WarehouseItemId, cancellationToken);

        if (item is null)
            return Result.Failure(WarehouseError.ItemNotFound(request.WarehouseItemId));

        return item.Reserve(request.Quantity);
    }
}