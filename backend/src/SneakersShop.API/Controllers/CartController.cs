using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SneakersShop.API.Contracts.Carts;
using SneakersShop.API.Extensions;
using SneakersShop.Application.Carts.Commands.AddCartItem;
using SneakersShop.Application.Carts.Commands.ChangeCartItemQuantity;
using SneakersShop.Application.Carts.Commands.RemoveCartItem;
using SneakersShop.Application.Carts.Queries.GetCart;

namespace SneakersShop.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public sealed class CartController(ISender sender) : ControllerBase
{
    [HttpPost("items")]
    public async Task<IActionResult> AddItem(
        [FromBody] AddCartItemCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetCartQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("items/{warehouseItemId:guid}")]
    public async Task<IActionResult> ChangeQuantity(
    Guid warehouseItemId,
    [FromBody] ChangeQuantityRequest body,
    CancellationToken cancellationToken = default)
    {
        var command = new ChangeCartItemQuantityCommand(warehouseItemId, body.Quantity);
        var result = await sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("items/{warehouseItemId:guid}")]
    public async Task<IActionResult> RemoveItem(
    Guid warehouseItemId,
    CancellationToken cancellationToken = default)
    {
        var command = new RemoveCartItemCommand(warehouseItemId);
        var result = await sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}