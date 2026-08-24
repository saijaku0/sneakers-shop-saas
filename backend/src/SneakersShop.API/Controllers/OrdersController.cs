using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SneakersShop.API.Extensions;
using SneakersShop.Application.Orders.Command.CreateOrder;

namespace SneakersShop.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
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
}