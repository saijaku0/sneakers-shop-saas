using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SneakersShop.Application.Catalog.Queries.GetProducts;

namespace SneakersShop.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] GetProductsQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }
}