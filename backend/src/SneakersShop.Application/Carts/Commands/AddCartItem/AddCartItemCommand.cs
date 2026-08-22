using SneakersShop.Application.Abstractions.Commands;

namespace SneakersShop.Application.Carts.Commands.AddCartItem;

public record AddCartItemCommand(Guid WarehouseItemId, int Quantity) : ICommand;