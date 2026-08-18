using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SneakersShop.Application.Catalog.Queries.GetProducts;
using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Catalog.Enums;
using SneakersShop.Domain.Catalog.ValueObjects;
using SneakersShop.Domain.Warehouse;
using SneakersShop.Domain.Warehouse.ValueObjects;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Catalog;

[Collection("IntegrationTests")]
public class GetProductsQueryTests(DatabaseFixture fixture)
{
    private static async Task CleanUpAsync(DatabaseFixture fixture)
    {
        await using var ctx = fixture.CreateDbContext();
        await ctx.WarehouseItems.ExecuteDeleteAsync();
        await ctx.ProductVariants.ExecuteDeleteAsync();
        await ctx.Products.ExecuteDeleteAsync();
        await ctx.Categories.ExecuteDeleteAsync();
        await ctx.Brands.ExecuteDeleteAsync();
    }

    private static async Task SeedCatalogAsync(DatabaseFixture fixture)
    {
        await using var ctx = fixture.CreateDbContext();

        var nike = Brand.Create("Nike");
        var adidas = Brand.Create("Adidas");
        var running = Category.Create("Running");
        var sneakers = Category.Create("Sneakers");

        ctx.Brands.AddRange(nike, adidas);
        ctx.Categories.AddRange(running, sneakers);

        var specs = new[]
        {
            (nike, running, Gender.Men, "Pegasus", 120m,"Black", 27.0m, 5),
            (nike, sneakers, Gender.Women, "AirForce", 110m, "White", 25.0m, 3),
            (adidas, running, Gender.Men, "Ultraboost", 180m, "Blue", 27.0m, 0),
            (adidas, sneakers, Gender.Unisex, "Samba", 90m, "Black", 26.0m, 7),
        };

        foreach (var (brand, cat, gender, model, price, color, sizeCm, qty) in specs)
        {
            var product = Product.Create(brand.Id, cat.Id, gender, model, $"{model} desc", price);
            var variant = ProductVariant.Create(product.Id, color, [new ProductImage("https://cdn.test/x.jpg")]);
            var warehouse = WarehouseItem.Create(variant.Id, new Size(sizeCm), qty == 0 ? 1 : qty).Value;
            if (qty == 0)
                warehouse.Reserve(1);

            ctx.Products.Add(product);
            ctx.ProductVariants.Add(variant);
            ctx.WarehouseItems.Add(warehouse);
        }

        await ctx.SaveChangesAsync();
    }

    private async Task<ISender> BuildSenderAndSeedAsync()
    {
        await CleanUpAsync(fixture);
        await SeedCatalogAsync(fixture);
        var provider = TestServiceProviderFactory.Build(fixture.ConnectionString);
        return provider.GetRequiredService<ISender>();
    }

    [Fact]
    public async Task NoFilters_ReturnsAllWithCorrectPagination()
    {
        var sender = await BuildSenderAndSeedAsync();

        var result = await sender.Send(new GetProductsQuery(null, null, null, PageSize: 2));

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(4);
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalPages.Should().Be(2);
        result.Value.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public async Task FilterByGender_NarrowsResults()
    {
        var sender = await BuildSenderAndSeedAsync();

        var result = await sender.Send(new GetProductsQuery(Gender.Men, null, null));

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(2);
        result.Value.Items.Should().OnlyContain(i => i.Model == "Pegasus" || i.Model == "Ultraboost");
    }

    [Fact]
    public async Task FilterByBrandName_ReturnsOnlyThatBrand()
    {
        var sender = await BuildSenderAndSeedAsync();

        var result = await sender.Send(new GetProductsQuery(null, null, "Nike"));

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(2);
        result.Value.Items.Should().OnlyContain(i => i.BrandName == "Nike");
    }

    [Fact]
    public async Task FilterByPriceRange_ReturnsWithinRange()
    {
        var sender = await BuildSenderAndSeedAsync();

        var result = await sender.Send(new GetProductsQuery(null, null, null, MinPrice: 100, MaxPrice: 130));

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(2);
        result.Value.Items.Should().OnlyContain(i => i.BasePrice >= 100 && i.BasePrice <= 130);
    }

    [Fact]
    public async Task FilterBySize_JoinsThroughVariantAndWarehouse()
    {
        var sender = await BuildSenderAndSeedAsync();

        var result = await sender.Send(new GetProductsQuery(null, null, null, Size: 27.0m));

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().OnlyContain(i => i.Model == "Pegasus");
        result.Value.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task InStockOnly_ExcludesUnavailable()
    {
        var sender = await BuildSenderAndSeedAsync();

        var result = await sender.Send(new GetProductsQuery(null, null, null, InStockOnly: true));

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(3);
        result.Value.Items.Should().NotContain(i => i.Model == "Ultraboost");
    }

    [Fact]
    public async Task Projection_IncludesPreviewImageAndColorCount()
    {
        var sender = await BuildSenderAndSeedAsync();

        var result = await sender.Send(new GetProductsQuery(null, null, "Nike"));

        var item = result.Value.Items.First();
        item.PreviewImageUrl.Should().Be("https://cdn.test/x.jpg");
        item.ColorCount.Should().Be(1);
    }
}