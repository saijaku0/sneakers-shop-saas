using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

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

public class RemoveCartItemTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public RemoveCartItemTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _authHelper = new TestAuthHelper(_client);
    }

    private async Task<Guid> SeedWarehouseItemAsync(int quantity)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var brand = Brand.Create($"Brand_{Guid.NewGuid()}");
        var category = Category.Create("Running");
        var product = Product.Create(brand.Id, category.Id, Gender.Men, "Model X", "desc", 100m);
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

    private async Task AddToCartAsync(string token, Guid warehouseItemId, int quantity)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cart/items")
        {
            Content = JsonContent.Create(new { warehouseItemId, quantity })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        (await _client.SendAsync(request)).StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task<CartDto> GetCartAsync(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/cart");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<CartDto>())!;
    }

    private HttpRequestMessage BuildDeleteRequest(string? token, Guid warehouseItemId)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Delete, $"/api/v1/cart/items/{warehouseItemId}");
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    [Fact]
    public async Task RemoveItem_DeletesFromCart()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 10);
        await AddToCartAsync(token, warehouseItemId, 2);

        var response = await _client.SendAsync(BuildDeleteRequest(token, warehouseItemId));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var cart = await GetCartAsync(token);
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveItem_LeavesOtherItemsIntact()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var itemA = await SeedWarehouseItemAsync(quantity: 10);
        var itemB = await SeedWarehouseItemAsync(quantity: 10);
        await AddToCartAsync(token, itemA, 2);
        await AddToCartAsync(token, itemB, 3);

        var response = await _client.SendAsync(BuildDeleteRequest(token, itemA));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var cart = await GetCartAsync(token);
        cart.Items.Should().ContainSingle();
        cart.Items.First().WarehouseItemId.Should().Be(itemB);
        cart.Items.First().Quantity.Should().Be(3);
    }

    [Fact]
    public async Task RemoveItem_NonexistentItem_ButCartExists_Succeeds()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 10);
        await AddToCartAsync(token, warehouseItemId, 1);

        var response = await _client.SendAsync(BuildDeleteRequest(token, Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var cart = await GetCartAsync(token);
        cart.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task RemoveItem_NoCart_Returns404()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();

        var response = await _client.SendAsync(BuildDeleteRequest(token, Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveItem_WithoutToken_Returns401()
    {
        var response = await _client.SendAsync(BuildDeleteRequest(token: null, Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}