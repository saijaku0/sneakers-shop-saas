using SneakersShop.Application.Abstractions.Commands;

namespace SneakersShop.Application.Warehouse.Commands.ReserveStock;

public sealed record ReserveStockCommand(Guid WarehouseItemId, int Quantity) : ICommand;