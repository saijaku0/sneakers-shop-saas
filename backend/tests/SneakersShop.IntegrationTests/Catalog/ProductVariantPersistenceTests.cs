using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using SneakersShop.Domain.Brands;
using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Catalog.Enums;
using SneakersShop.Domain.Catalog.ValueObjects;
using SneakersShop.IntegrationTests.Infrastructure;

namespace SneakersShop.IntegrationTests.Catalog;

[Collection("IntegrationTests")]
public class ProductVariantPersistenceTests(DatabaseFixture fixture)
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

    [Fact]
    public async Task Images_RoundTrip_UrlsSurviveReadFromDatabase()
    {
        await CleanUpAsync(fixture);

        var urls = new[]
        {
            "https://cdn.test/black-1.jpg",
            "https://cdn.test/black-2.jpg",
            "https://cdn.test/black-3.jpg",
            "https://cdn.test/black-4.jpg",
            "https://cdn.test/black-5.jpg",
            "https://cdn.test/black-6.jpg",
            "https://cdn.test/black-7.jpg",
        };

        Guid variantId;

        await using (var write = fixture.CreateDbContext())
        {
            var brand = Brand.Create("Nike");
            var category = Category.Create("Running");
            var product = Product.Create(brand.Id, category.Id, Gender.Men, "Pegasus", "desc", 120m);
            var variant = ProductVariant.Create(
                product.Id,
                "Black",
                urls.Select(u => new ProductImage(u)));

            variantId = variant.Id;

            write.Brands.Add(brand);
            write.Categories.Add(category);
            write.Products.Add(product);
            write.ProductVariants.Add(variant);
            await write.SaveChangesAsync();
        }

        await using var read = fixture.CreateDbContext();
        var loaded = await read.ProductVariants
            .AsNoTracking()
            .FirstAsync(v => v.Id == variantId);

        loaded.Images.Should().HaveCount(7);
        loaded.Images.Select(i => i.Url).Should().BeEquivalentTo(urls);
        loaded.Images.Should().OnlyContain(i => !string.IsNullOrWhiteSpace(i.Url));
    }
}