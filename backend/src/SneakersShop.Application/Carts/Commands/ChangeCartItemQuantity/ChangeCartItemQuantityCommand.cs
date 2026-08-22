using SneakersShop.Application.Abstractions.Commands;

namespace SneakersShop.Application.Carts.Commands.ChangeCartItemQuantity;

public sealed record ChangeCartItemQuantityCommand(Guid WarehouseItemId, int Quantity) : ICommand;