using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Carts.DTOs;
using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Catalog.Enums;
using SneakersShop.Domain.Catalog.ValueObjects;
using SneakersShop.Domain.Warehouse;
using SneakersShop.Domain.Warehouse.ValueObjects;
using SneakersShop.Infrastructure.Persistence;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Carts;

public class GetCartTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public GetCartTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _authHelper = new TestAuthHelper(_client);
    }

    private async Task<Guid> SeedWarehouseItemAsync(int quantity, decimal price = 100m)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var brand = Brand.Create($"Brand_{Guid.NewGuid()}");
        var category = Category.Create("Running");
        var product = Product.Create(brand.Id, category.Id, Gender.Men, "Model X", "desc", price);
        var variant = ProductVariant.Create(
            product.Id, "Black", [new ProductImage("https://test.com/x.jpg")]);
        var warehouseItem = WarehouseItem.Create(variant.Id, new Size(23m), quantity).Value;

        context.Brands.Add(brand);
        context.Categories.Add(category);
        context.Products.Add(product);
        context.ProductVariants.Add(variant);
        context.WarehouseItems.Add(warehouseItem);
        await context.SaveChangesAsync();

        return warehouseItem.Id;
    }

    private async Task SetAvailableAsync(Guid warehouseItemId, int targetAvailable)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var item = await context.WarehouseItems.FirstAsync(w => w.Id == warehouseItemId);
        var toReserve = item.Quantity - targetAvailable;
        item.Reserve(toReserve);
        await context.SaveChangesAsync();
    }

    private async Task AddToCartAsync(string token, Guid warehouseItemId, int quantity)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cart/items")
        {
            Content = JsonContent.Create(new { warehouseItemId, quantity })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private HttpRequestMessage BuildGetCartRequest(string? token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/cart");
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    [Fact]
    public async Task GetCart_WithItems_ReturnsLivePriceAndTotal()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5, price: 140m);
        await AddToCartAsync(token, warehouseItemId, 2);

        var response = await _client.SendAsync(BuildGetCartRequest(token));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();

        cart.Should().NotBeNull();
        cart!.Items.Should().ContainSingle();
        var item = cart.Items.First();
        item.WarehouseItemId.Should().Be(warehouseItemId);
        item.UnitPrice.Should().Be(140m);
        item.Quantity.Should().Be(2);
        item.Available.Should().Be(5);
        item.IsAvailable.Should().BeTrue();
        cart.TotalPrice.Should().Be(280m);
    }

    [Fact]
    public async Task GetCart_WhenStockDroppedBelowQuantity_MarksItemUnavailable()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5);
        await AddToCartAsync(token, warehouseItemId, 5);

        await SetAvailableAsync(warehouseItemId, targetAvailable: 2);

        var response = await _client.SendAsync(BuildGetCartRequest(token));

        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        var item = cart!.Items.First();
        item.Available.Should().Be(2);
        item.Quantity.Should().Be(5);
        item.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task GetCart_EmptyCart_Returns200WithEmptyItems()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();

        var response = await _client.SendAsync(BuildGetCartRequest(token));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cart = await response.Content.ReadFromJsonAsync<CartDto>();
        cart!.Items.Should().BeEmpty();
        cart.TotalPrice.Should().Be(0m);
    }

    [Fact]
    public async Task GetCart_IsScopedToCurrentUser()
    {
        var tokenA = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5);
        await AddToCartAsync(tokenA, warehouseItemId, 2);

        var authHelperB = new TestAuthHelper(_client);
        var tokenB = await authHelperB.GetValidAccessTokenAsync();

        var response = await _client.SendAsync(BuildGetCartRequest(tokenB));

        var cartB = await response.Content.ReadFromJsonAsync<CartDto>();
        cartB!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCart_WithoutToken_Returns401()
    {
        var response = await _client.SendAsync(BuildGetCartRequest(token: null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}