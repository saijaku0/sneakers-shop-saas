using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

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

namespace SneakersShop.IntegrationTests.Carts;

public class AddCartItemTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly TestAuthHelper _authHelper;

    public AddCartItemTests(CustomWebApplicationFactory<Program> factory)
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

    private static Guid ExtractUserId(string accessToken)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var sub = jwt.Claims.FirstOrDefault(c =>
                      c.Type == ClaimTypes.NameIdentifier || c.Type == "sub" || c.Type == "nameid")
                  ?? throw new InvalidOperationException("No user id claim in token.");
        return Guid.Parse(sub.Value);
    }

    private HttpRequestMessage BuildAddItemRequest(string token, Guid warehouseItemId, int quantity)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cart/items")
        {
            Content = JsonContent.Create(new { warehouseItemId, quantity })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    [Fact]
    public async Task AddItem_WithValidRequest_Returns204AndPersistsItem()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var userId = ExtractUserId(token);
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5);

        var response = await _client.SendAsync(
            BuildAddItemRequest(token, warehouseItemId, 2));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var cart = await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        cart.Should().NotBeNull();
        cart!.Items.Should().ContainSingle();
        cart.Items.First().WarehouseItemId.Should().Be(warehouseItemId);
        cart.Items.First().Quantity.Should().Be(2);
    }

    [Fact]
    public async Task AddItem_SameItemTwice_SumsQuantity()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var userId = ExtractUserId(token);
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 10);

        (await _client.SendAsync(BuildAddItemRequest(token, warehouseItemId, 2)))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await _client.SendAsync(BuildAddItemRequest(token, warehouseItemId, 3)))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var cart = await context.Carts
            .Include(c => c.Items)
            .FirstAsync(c => c.UserId == userId);

        cart.Items.Should().ContainSingle();
        cart.Items.First().Quantity.Should().Be(5);
    }

    [Fact]
    public async Task AddItem_NonexistentWarehouseItem_Returns404()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();

        var response = await _client.SendAsync(
            BuildAddItemRequest(token, Guid.NewGuid(), 1));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddItem_QuantityExceedsAvailable_ReturnsConflictOrBadRequest()
    {
        var token = await _authHelper.GetValidAccessTokenAsync();
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 1);

        var response = await _client.SendAsync(
            BuildAddItemRequest(token, warehouseItemId, 5));

        response.StatusCode.Should().BeOneOf(HttpStatusCode.Conflict, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddItem_WithoutToken_Returns401()
    {
        var warehouseItemId = await SeedWarehouseItemAsync(quantity: 5);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cart/items")
        {
            Content = JsonContent.Create(new { warehouseItemId, quantity = 1 })
        };

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}