using MediatR;

using Microsoft.AspNetCore.Mvc;

using SneakersShop.API.Contracts.Warehouse;
using SneakersShop.API.Extensions;
using SneakersShop.Application.Warehouse.Commands.ReserveStock;

namespace SneakersShop.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WarehouseItemsController(ISender sender) : ControllerBase
{
    [HttpPost("{id:guid}/reserve")]
    public async Task<IActionResult> Reserve(
        Guid id,
        [FromBody] ReserveStockRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ReserveStockCommand(id, request.Quantity), cancellationToken);

        return result.ToActionResult();
    }
}