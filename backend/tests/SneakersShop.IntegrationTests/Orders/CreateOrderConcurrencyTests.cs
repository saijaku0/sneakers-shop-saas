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
using SneakersShop.Domain.Warehouse;
using SneakersShop.Domain.Warehouse.ValueObjects;
using SneakersShop.Infrastructure.Persistence;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Orders;

public class CreateOrderConcurrencyTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CreateOrderConcurrencyTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private async Task<Guid> SeedSingleUnitWarehouseItemAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var brand = Brand.Create($"Brand_{Guid.NewGuid()}");
        var category = Category.Create("Running");
        var product = Product.Create(brand.Id, category.Id, Gender.Men, "Model X", "desc", 100m);
        var variant = ProductVariant.Create(
            product.Id, "Black", [new ProductImage("https://test.com/x.jpg")]);
        var warehouseItem = WarehouseItem.Create(variant.Id, new Size(23m), 1).Value;

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

    private async Task<Func<Task<HttpResponseMessage>>> PrepareCheckoutAsync(Guid warehouseItemId)
    {
        var client = _factory.CreateClient();
        var auth = new TestAuthHelper(client);
        var token = await auth.GetValidAccessTokenAsync();

        var addToCart = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cart/items")
        {
            Content = JsonContent.Create(new { warehouseItemId, quantity = 1 })
        };
        addToCart.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        (await client.SendAsync(addToCart)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        return () =>
        {
            var order = new HttpRequestMessage(HttpMethod.Post, "/api/v1/orders")
            {
                Content = JsonContent.Create(new { shippingAddress = SampleAddress(), paymentMethod = 0 })
            };
            order.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client.SendAsync(order);
        };
    }

    [Fact]
    public async Task ParallelCheckouts_OnLastUnit_OnlyOneSucceeds_NoOversell()
    {
        const int parallelUsers = 10;
        var warehouseItemId = await SeedSingleUnitWarehouseItemAsync(); // Quantity = 1

        var checkouts = new List<Func<Task<HttpResponseMessage>>>();
        for (int i = 0; i < parallelUsers; i++)
            checkouts.Add(await PrepareCheckoutAsync(warehouseItemId));

        var responses = await Task.WhenAll(checkouts.Select(c => c()));

        responses.Count(r => r.StatusCode == HttpStatusCode.OK).Should().Be(1);
        responses.Count(r => r.StatusCode != HttpStatusCode.OK).Should().Be(parallelUsers - 1);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var item = await context.WarehouseItems.FirstAsync(w => w.Id == warehouseItemId);
        item.ReservedQuantity.Should().Be(1);
        (item.Quantity - item.ReservedQuantity).Should().Be(0);

        var orderCount = await context.Orders
            .SelectMany(o => o.OrderItems)
            .CountAsync(oi => oi.WarehouseItemId == warehouseItemId);
        orderCount.Should().Be(1);
    }
}