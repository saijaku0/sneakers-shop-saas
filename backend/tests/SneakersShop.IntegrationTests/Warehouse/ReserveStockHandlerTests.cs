using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Warehouse.Commands.ReserveStock;
using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Catalog.Enums;
using SneakersShop.Domain.Catalog.ValueObjects;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Warehouse;
using SneakersShop.Domain.Warehouse.ValueObjects;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Warehouse;

[Collection("IntegrationTests")]
public class ReserveStockHandlerTests(DatabaseFixture fixtureData)
{
    // Test data
    private static async Task<Guid> SeedTestDataAsync(DatabaseFixture fixture)
    {
        await using var context = fixture.CreateDbContext();

        List<string> imagesUrls =
        [
            "https://test.com/image1.jpg",
        "https://test.com/image2.jpg"
        ];
        IEnumerable<ProductImage> productImages = imagesUrls.Select(url => new ProductImage(url));

        Size size = new(23);

        var brand = Brand.Create("Test Brand");
        var category = Category.Create("Running");

        var product = Product.Create(
            brand.Id,
            category.Id,
            Gender.Men,
            "Model X",
            "Short Description",
            100m);

        var variant = ProductVariant.Create(
            product.Id,
            "Black",
            productImages);

        var warehouseItem = WarehouseItem.Create(variant.Id, size, 1).Value;

        context.Brands.Add(brand);
        context.Categories.Add(category);
        context.Products.Add(product);
        context.ProductVariants.Add(variant);
        context.WarehouseItems.Add(warehouseItem);
        await context.SaveChangesAsync();

        return warehouseItem.Id;
    }

    // Clean up test data
    private static async Task CleanUpTestDataAsync(
        DatabaseFixture fixture,
        CancellationToken cancellationToken = default)
    {
        await using var context = fixture.CreateDbContext();
        await context.WarehouseItems.ExecuteDeleteAsync(cancellationToken);
        await context.Products.ExecuteDeleteAsync(cancellationToken);
        await context.Brands.ExecuteDeleteAsync(cancellationToken);
    }

    [Fact]
    public async Task ReserveStockCommandHandler_ShouldReserveStock_WhenItemExistsAndSufficientQuantity()
    {
        await CleanUpTestDataAsync(fixtureData);
        var warehouseItemId = await SeedTestDataAsync(fixtureData);

        var result = await ReserveStockAsync(warehouseItemId);

        // Assert: result should be successful
        result.IsSuccess.Should().BeTrue();

        await using var assertContext = fixtureData.CreateDbContext();
        var updatedItem = await assertContext.WarehouseItems.FindAsync([warehouseItemId], CancellationToken.None);

        updatedItem.Should().NotBeNull();
        updatedItem!.Quantity.Should().Be(1); // The total quantity remains the same
        updatedItem.ReservedQuantity.Should().Be(1); // The reserved quantity should be updated
        updatedItem.Available.Should().Be(0); // The available quantity should be 0 after reserving 1 out of 1
    }

    [Fact]
    public async Task ReserveStockCommandHandler_OnlyOneSucceeds_WhenManyParallelRequestsForSameItem()
    {
        await CleanUpTestDataAsync(fixtureData);
        var contestedWarehouseItemId = await SeedTestDataAsync(fixtureData);

        const int parallelRequests = 10;
        Task<Result>[] tasks = [.. Enumerable.Range(0, parallelRequests)
        .Select(_ => ReserveStockAsync(contestedWarehouseItemId))];

        Result[] results = await Task.WhenAll(tasks);

        // One request should succeed, the others should fail due to insufficient stock
        results.Count(r => r.IsSuccess).Should().Be(1);
        results.Count(r => r.IsFailure).Should().Be(parallelRequests - 1);

        // In the end, the reserved quantity should be 1 and available quantity should be 0
        await using var assertContext = fixtureData.CreateDbContext();
        var item = await assertContext.WarehouseItems.FindAsync([contestedWarehouseItemId], CancellationToken.None);

        item.Should().NotBeNull();
        item!.ReservedQuantity.Should().Be(1);
        item.Available.Should().Be(0);
    }

    private async Task<Result> ReserveStockAsync(Guid warehouseItemId)
    {
        IServiceProvider serviceProvider = TestServiceProviderFactory.Build(fixtureData.ConnectionString);
        ISender mediator = serviceProvider.GetRequiredService<ISender>();
        var command = new ReserveStockCommand(warehouseItemId, 1);
        return await mediator.Send(command);
    }
}