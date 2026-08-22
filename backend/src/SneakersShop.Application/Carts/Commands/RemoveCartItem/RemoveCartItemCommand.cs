using SneakersShop.Application.Abstractions.Commands;

namespace SneakersShop.Application.Carts.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(Guid WarehouseItemId) : ICommand;