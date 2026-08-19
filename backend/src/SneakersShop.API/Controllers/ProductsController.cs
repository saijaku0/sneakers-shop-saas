using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SneakersShop.API.Extensions;
using SneakersShop.Application.Catalog.Queries.GetProductItem;
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
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductItem(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductItemQuery(id);
        var result = await sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}