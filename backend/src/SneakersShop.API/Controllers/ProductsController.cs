using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SneakersShop.API.Extensions;
using SneakersShop.Application.Brands.Queries.GetBrands;
using SneakersShop.Application.Catalog.Queries.GetFilters;
using SneakersShop.Application.Catalog.Queries.GetProductItem;
using SneakersShop.Application.Catalog.Queries.GetProducts;
using SneakersShop.Application.Categories.Queries.GetCategories;

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

    [HttpGet("filters")]
    public async Task<IActionResult> GetFilters(CancellationToken cancellationToken)
    {
        var query = new GetFiltersQuery();
        var result = await sender.Send(query, cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands(CancellationToken cancellation)
    {
        var query = new GetBrandsQuery();
        var result = await sender.Send(query, cancellation);

        return result.ToActionResult();
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellation)
    {
        var query = new GetCategoriesQuery();
        var result = await sender.Send(query, cancellation);

        return result.ToActionResult();
    }
}