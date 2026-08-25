using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Orders.DTOs;
using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Catalog.Enums;
using SneakersShop.Domain.Catalog.ValueObjects;
using SneakersShop.Domain.Warehouse;
using SneakersShop.Domain.Warehouse.ValueObjects;
using SneakersShop.Infrastructure.Persistence;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Orders;

public class GetOrdersTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public GetOrdersTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _authHelper = new TestAuthHelper(_client);
    }

    private async Task<Guid> SeedWarehouseItemAsync(int quantity, decimal price)
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

    private static object SampleAddress() => new
    {
        country = "Germany",
        state = (string?)null,
        city = "Nuremberg",
        street = "Hauptstrasse",
        houseNumber = "10",
        zipCode = "90402"
    };

    private async Task<Guid> PlaceOrderAsync(string token, Guid warehouseItemId, int quantity)
    {
        var addToCart = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cart/items")
        {
            Content = JsonContent.Create(new { warehouseItemId, quantity })
        };
        addToCart.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        (await _client.SendAsync(addToCart)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        var createOrder = new HttpRequestMessage(HttpMethod.Post, "/api/v1/orders")
        {
            Content = JsonContent.Create(new { shippingAddress = SampleAddress(), paymentMethod = 0 })
        };
        createOrder.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(createOrder);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private HttpRequestMessage BuildGet(string? token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/orders");
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    [Fact]
    public async Task GetOrders_ReturnsOnlyOwnOrders()
    {
        var tokenA = await _authHelper.GetValidAccessTokenAsync();
        var w1 = await SeedWarehouseItemAsync(quantity: 5, price: 100m);
        var w2 = await SeedWarehouseItemAsync(quantity: 5, price: 150m);
        var orderA1 = await PlaceOrderAsync(tokenA, w1, 1);
        var orderA2 = await PlaceOrderAsync(tokenA, w2, 2);

        var authB = new TestAuthHelper(_client);
        var tokenB = await authB.GetValidAccessTokenAsync();
        var w3 = await SeedWarehouseItemAsync(quantity: 5, price: 200m);
        var orderB = await PlaceOrderAsync(tokenB, w3, 1);

        var response = await _client.SendAsync(BuildGet(tokenA));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderSummaryDto>>();

        orders.Should().HaveCount(2);
        orders!.Select(o => o.Id).Should().BeEquivalentTo([orderA1, orderA2]);
        orders.Should().NotContain(o => o.Id == orderB);
    }

    [Fact]
    public async Task GetOrders_EmptyWhenNoOrders_Returns200()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();

        var response = await _client.SendAsync(BuildGet(token));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderSummaryDto>>();
        orders.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOrders_NewestFirst()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var w1 = await SeedWarehouseItemAsync(quantity: 5, price: 100m);
        var w2 = await SeedWarehouseItemAsync(quantity: 5, price: 100m);

        var first = await PlaceOrderAsync(token, w1, 1);
        await Task.Delay(50);
        var second = await PlaceOrderAsync(token, w2, 1);

        var response = await _client.SendAsync(BuildGet(token));
        var orders = await response.Content.ReadFromJsonAsync<List<OrderSummaryDto>>();

        orders.Should().HaveCount(2);
        orders![0].Id.Should().Be(second);
        orders[1].Id.Should().Be(first);
    }

    [Fact]
    public async Task GetOrders_SummaryFieldsArePopulated()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var w = await SeedWarehouseItemAsync(quantity: 5, price: 100m);
        var orderId = await PlaceOrderAsync(token, w, 2);

        var response = await _client.SendAsync(BuildGet(token));
        var orders = await response.Content.ReadFromJsonAsync<List<OrderSummaryDto>>();

        var summary = orders!.Single(o => o.Id == orderId);
        summary.Status.Should().Be("Paid");
        summary.TotalAmount.Should().Be(200m);
        summary.ItemCount.Should().Be(1);
    }

    [Fact]
    public async Task GetOrders_WithoutToken_Returns401()
    {
        var response = await _client.SendAsync(BuildGet(token: null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}