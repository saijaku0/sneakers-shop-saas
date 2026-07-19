using FluentAssertions;

using SneakersShop.Domain.Brands;

namespace SneakersShop.Domain.UnitTests.Brands;

public class BrandTests
{
    [Fact]
    public void Create_WithValidName_ReturnsBrand()
    {
        var brand = Brand.Create("Nike");

        brand.Name.Should().Be("Nike");
        brand.Id.Should().NotBe(Guid.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankName_Throws(string name)
    {
        var act = () => Brand.Create(name);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateName_ChangesName()
    {
        var brand = Brand.Create("Nike");

        brand.UpdateName("Adidas");

        brand.Name.Should().Be("Adidas");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithBlankName_Throws(string name)
    {
        var brand = Brand.Create("Nike");

        var act = () => brand.UpdateName(name);
        act.Should().Throw<ArgumentException>();
    }
}