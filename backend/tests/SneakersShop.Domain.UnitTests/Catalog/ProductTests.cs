using FluentAssertions;

using SneakersShop.Domain.Catalog;
using SneakersShop.Domain.Catalog.DomainEvents;
using SneakersShop.Domain.Catalog.Enums;

namespace SneakersShop.Domain.UnitTests.Catalog;

public class ProductTests
{
    private static Product CreateProduct()
        => Product.Create(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Gender.Men,
            "Air Max 90",
            "Classic runner",
            120m);

    [Fact]
    public void Create_WithValidData_ReturnsActiveProduct()
    {
        var brandId = Guid.CreateVersion7();
        var categoryId = Guid.CreateVersion7();

        var product = Product.Create(brandId, categoryId, Gender.Men, "Air Max 90", "Classic runner", 120m);

        product.BrandId.Should().Be(brandId);
        product.CategoryId.Should().Be(categoryId);
        product.Gender.Should().Be(Gender.Men);
        product.Model.Should().Be("Air Max 90");
        product.Description.Should().Be("Classic runner");
        product.BasePrice.Should().Be(120m);
        product.IsActive.Should().BeTrue();
        product.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithEmptyBrandId_Throws()
    {
        var act = () => Product.Create(Guid.Empty, Guid.CreateVersion7(), Gender.Men, "M", "D", 100m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyCategoryId_Throws()
    {
        var act = () => Product.Create(Guid.CreateVersion7(), Guid.Empty, Gender.Men, "M", "D", 100m);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankModel_Throws(string model)
    {
        var act = () => Product.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), Gender.Men, model, "D", 100m);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankDescription_Throws(string description)
    {
        var act = () => Product.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), Gender.Men, "M", description, 100m);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositivePrice_Throws(decimal price)
    {
        var act = () => Product.Create(Guid.CreateVersion7(), Guid.CreateVersion7(), Gender.Men, "M", "D", price);
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