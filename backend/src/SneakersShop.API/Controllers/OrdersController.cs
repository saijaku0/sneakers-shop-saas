using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SneakersShop.API.Extensions;
using SneakersShop.Application.Orders.Command.CreateOrder;
using SneakersShop.Application.Orders.Queries.GetOrderById;
using SneakersShop.Application.Orders.Queries.GetOrders;

namespace SneakersShop.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetOrderByIdQuery(id), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetOrdersQuery(), cancellationToken);
        return result.ToActionResult();
    }
}