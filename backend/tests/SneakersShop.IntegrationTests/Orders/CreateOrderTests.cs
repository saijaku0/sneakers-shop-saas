using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Catalog.Enums;
using SneakersShop.Domain.Catalog.ValueObjects;
using SneakersShop.Domain.Orders.Enums;
using SneakersShop.Domain.Warehouse;
using SneakersShop.Domain.Warehouse.ValueObjects;
using SneakersShop.Infrastructure.Persistence;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Orders;

public class CreateOrderTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public CreateOrderTests(CustomWebApplicationFactory<Program> factory)
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

    private async Task AddToCartAsync(string token, Guid warehouseItemId, int quantity)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cart/items")
        {
            Content = JsonContent.Create(new { warehouseItemId, quantity })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        (await _client.SendAsync(request)).StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private HttpRequestMessage BuildCreateOrderRequest(string? token, object? shippingAddress, int paymentMethod = 0)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/orders")
        {
            Content = JsonContent.Create(new { shippingAddress, paymentMethod })
        };
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
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

    private async Task<string> GetTokenForUserWithoutAddressAsync()
    {
        var email = $"user_{Guid.NewGuid()}@example.com";
        const string password = "Password123!";

        var register = await _client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            name = "Ivan",
            lastName = "Petrov",
            phoneNumber = "+491234567890",
            email,
            password,
            defaultShippingAddress = (object?)null
        });
        register.EnsureSuccessStatusCode();

        var login = await _client.PostAsJsonAsync("/api/v1/auth/login", new { Email = email, Password = password });
        login.EnsureSuccessStatusCode();

        var content = await login.Content.ReadAsStringAsync();
        using var doc = System.Text.Json.JsonDocument.Parse(content);
        return doc.RootElement.GetProperty("accessToken").GetString()!;
    }

    [Fact]
    public async Task CreateOrder_WithItemsAndAddress_Succeeds_ReservesStock_ClearsCart()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5, price: 100m);
        await AddToCartAsync(token, warehouseItemId, 2);

        var response = await _client.SendAsync(
            BuildCreateOrderRequest(token, SampleAddress()));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orderId = await response.Content.ReadFromJsonAsync<Guid>();
        orderId.Should().NotBe(Guid.Empty);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        order.Should().NotBeNull();
        order!.Status.Should().Be(OrderStatus.Paid);
        order.TotalAmount.Should().Be(200m);
        order.OrderItems.Should().ContainSingle();

        var warehouse = await context.WarehouseItems.FirstAsync(w => w.Id == warehouseItemId);
        warehouse.ReservedQuantity.Should().Be(2);

        var cart = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == order.UserId);
        cart!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateOrder_EmptyCart_Fails()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();

        var response = await _client.SendAsync(
            BuildCreateOrderRequest(token, SampleAddress()));

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound, HttpStatusCode.BadRequest, HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateOrder_NoAddressAnywhere_Fails()
    {
        var token = await GetTokenForUserWithoutAddressAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5);
        await AddToCartAsync(token, warehouseItemId, 1);

        var response = await _client.SendAsync(
            BuildCreateOrderRequest(token, shippingAddress: null));

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest, HttpStatusCode.Conflict, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_InsufficientStock_Fails()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5);
        await AddToCartAsync(token, warehouseItemId, 3);

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var item = await context.WarehouseItems.FirstAsync(w => w.Id == warehouseItemId);
            item.Reserve(item.Quantity);
            await context.SaveChangesAsync();
        }

        var response = await _client.SendAsync(
            BuildCreateOrderRequest(token, SampleAddress()));

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Conflict, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_WithoutToken_Returns401()
    {
        var response = await _client.SendAsync(
            BuildCreateOrderRequest(token: null, SampleAddress()));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}