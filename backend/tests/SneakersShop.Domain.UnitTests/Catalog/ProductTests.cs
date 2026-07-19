using FluentAssertions;

using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Catalog.DomainEvents;
using SneakersShop.Domain.Catalog.ValueObjects;

namespace SneakersShop.Domain.UnitTests.Catalog;

public class ProductTests
{
    private static ProductImage Image(string url = "https://cdn.example.com/a.jpg")
        => new(url);

    private static Product CreateProduct()
        => Product.Create(
            Guid.CreateVersion7(),
            "Air Max 90",
            "Classic runner",
            120m,
            [Image()]);

    [Fact]
    public void Create_WithValidData_ReturnsActiveProduct()
    {
        var brandId = Guid.CreateVersion7();

        var product = Product.Create(brandId, "Air Max 90", "Classic runner", 120m, [Image()]);

        product.BrandId.Should().Be(brandId);
        product.Model.Should().Be("Air Max 90");
        product.Description.Should().Be("Classic runner");
        product.Price.Should().Be(120m);
        product.IsActive.Should().BeTrue();
        product.Images.Should().ContainSingle();
        product.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_CopiesImages_CallerListDoesNotLeakIn()
    {
        var images = new List<ProductImage> { Image() };

        var product = Product.Create(Guid.CreateVersion7(), "M", "D", 100m, images);
        images.Add(Image("https://cdn.example.com/b.jpg"));

        product.Images.Should().ContainSingle();
    }

    [Fact]
    public void Create_WithEmptyBrandId_Throws()
    {
        var act = () => Product.Create(Guid.Empty, "M", "D", 100m, [Image()]);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankModel_Throws(string model)
    {
        var act = () => Product.Create(Guid.CreateVersion7(), model, "D", 100m, [Image()]);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankDescription_Throws(string description)
    {
        var act = () => Product.Create(Guid.CreateVersion7(), "M", description, 100m, [Image()]);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositivePrice_Throws(decimal price)
    {
        var act = () => Product.Create(Guid.CreateVersion7(), "M", "D", price, [Image()]);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullImages_Throws()
    {
        var act = () => Product.Create(Guid.CreateVersion7(), "M", "D", 100m, null!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_WhenActive_SetsInactiveAndRaisesEvent()
    {
        var product = CreateProduct();

        product.Deactivate();

        product.IsActive.Should().BeFalse();
        product.DomainEvents.Should().ContainSingle(e => e is ProductDeactivated);
    }

    [Fact]
    public void Activate_WhenInactive_SetsActiveAndRaisesEvent()
    {
        var product = CreateProduct();
        product.Deactivate();
        product.ClearDomainEvents();

        product.Activate();

        product.IsActive.Should().BeTrue();
        product.DomainEvents.Should().ContainSingle(e => e is ProductActivated);
    }

    [Fact]
    public void Deactivate_Twice_RaisesEventOnlyOnce()
    {
        var product = CreateProduct();
        product.Deactivate();
        product.ClearDomainEvents();

        product.Deactivate();

        product.IsActive.Should().BeFalse();
        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Activate_WhenAlreadyActive_DoesNothing()
    {
        var product = CreateProduct();

        product.Activate();

        product.IsActive.Should().BeTrue();
        product.DomainEvents.Should().BeEmpty();
    }
}