using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
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

public class GetOrderByIdTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public GetOrderByIdTests(CustomWebApplicationFactory<Program> factory)
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

        return (await response.Content.ReadFromJsonAsync<Guid>());
    }

    private HttpRequestMessage BuildGet(string? token, Guid orderId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/orders/{orderId}");
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private async Task SetProductPriceAsync(Guid warehouseItemId, decimal newPrice)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var warehouse = await context.WarehouseItems.FirstAsync(w => w.Id == warehouseItemId);
        var variant = await context.ProductVariants.FirstAsync(v => v.Id == warehouse.ProductVariantId);
        var product = await context.Products.FirstAsync(p => p.Id == variant.ProductId);

        context.Entry(product).Property("BasePrice").CurrentValue = newPrice;
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetOrder_OwnOrder_ReturnsFullDetail()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5, price: 100m);
        var orderId = await PlaceOrderAsync(token, warehouseItemId, 2);

        var response = await _client.SendAsync(BuildGet(token, orderId));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDetailDto>();

        order.Should().NotBeNull();
        order!.Id.Should().Be(orderId);
        order.Status.Should().Be("Paid");
        order.TotalAmount.Should().Be(200m);
        order.ShippingAddress.City.Should().Be("Nuremberg");

        order.Items.Should().ContainSingle();
        var item = order.Items.First();
        item.WarehouseItemId.Should().Be(warehouseItemId);
        item.Model.Should().Be("Model X");
        item.BrandName.Should().NotBeNullOrEmpty();
        item.Color.Should().Be("Black");
        item.PreviewImageUrl.Should().NotBeNullOrEmpty();
        item.UnitPrice.Should().Be(100m);
        item.Quantity.Should().Be(2);
        item.TotalPrice.Should().Be(200m);
    }

    [Fact]
    public async Task GetOrder_PriceIsSnapshot_NotLiveCatalogPrice()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5, price: 100m);
        var orderId = await PlaceOrderAsync(token, warehouseItemId, 1);

        await SetProductPriceAsync(warehouseItemId, newPrice: 999m);

        var response = await _client.SendAsync(BuildGet(token, orderId));
        var order = await response.Content.ReadFromJsonAsync<OrderDetailDto>();

        order!.Items.First().UnitPrice.Should().Be(100m);
        order.TotalAmount.Should().Be(100m);
    }

    [Fact]
    public async Task GetOrder_OtherUsersOrder_Returns404()
    {
        var tokenA = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5, price: 100m);
        var orderId = await PlaceOrderAsync(tokenA, warehouseItemId, 1);

        var authHelperB = new TestAuthHelper(_client);
        var tokenB = await authHelperB.GetValidAccessTokenAsync();

        var response = await _client.SendAsync(BuildGet(tokenB, orderId));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrder_NonexistentId_Returns404()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();

        var response = await _client.SendAsync(BuildGet(token, Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrder_WithoutToken_Returns401()
    {
        var response = await _client.SendAsync(BuildGet(token: null, Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}