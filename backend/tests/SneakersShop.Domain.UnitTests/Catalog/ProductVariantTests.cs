using FluentAssertions;

using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Catalog.ValueObjects;

namespace SneakersShop.Domain.UnitTests.Catalog;

public class ProductVariantTests
{
    private static ProductImage Image(string url = "https://cdn.example.com/a.jpg")
        => new(url);

    private static ProductVariant CreateVariant()
        => ProductVariant.Create(Guid.CreateVersion7(), "Black", [Image()]);

    [Fact]
    public void Create_WithValidData_ReturnsVariant()
    {
        var productId = Guid.CreateVersion7();

        var variant = ProductVariant.Create(productId, "Black/Volt", [Image()]);

        variant.ProductId.Should().Be(productId);
        variant.Color.Should().Be("Black/Volt");
        variant.Images.Should().ContainSingle();
        variant.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_CopiesImages_CallerListDoesNotLeakIn()
    {
        var images = new List<ProductImage> { Image() };

        var variant = ProductVariant.Create(Guid.CreateVersion7(), "Black", images);
        images.Add(Image("https://cdn.example.com/b.jpg"));

        variant.Images.Should().ContainSingle();
    }

    [Fact]
    public void Create_WithEmptyProductId_Throws()
    {
        var act = () => ProductVariant.Create(Guid.Empty, "Black", [Image()]);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankColor_Throws(string color)
    {
        var act = () => ProductVariant.Create(Guid.CreateVersion7(), color, [Image()]);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullImages_Throws()
    {
        var act = () => ProductVariant.Create(Guid.CreateVersion7(), "Black", null!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyImages_Throws()
    {
        var act = () => ProductVariant.Create(Guid.CreateVersion7(), "Black", []);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateColor_WithValidColor_ChangesColor()
    {
        var variant = CreateVariant();

        variant.UpdateColor("White");

        variant.Color.Should().Be("White");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateColor_WithBlank_Throws(string color)
    {
        var variant = CreateVariant();

        var act = () => variant.UpdateColor(color);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetImages_ReplacesImages()
    {
        var variant = CreateVariant();

        variant.SetImages([Image("https://cdn.example.com/x.jpg"), Image("https://cdn.example.com/y.jpg")]);

        variant.Images.Should().HaveCount(2);
    }

    [Fact]
    public void SetImages_WithEmpty_Throws()
    {
        var variant = CreateVariant();

        var act = () => variant.SetImages([]);

        act.Should().Throw<ArgumentException>();
    }
}