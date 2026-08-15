using FluentAssertions;

using SneakersShop.Domain.Catalog;

namespace SneakersShop.Domain.UnitTests.Catalog;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidName_ReturnsCategory()
    {
        var category = Category.Create("Running");

        category.Name.Should().Be("Running");
        category.Id.Should().NotBe(Guid.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankName_Throws(string name)
    {
        var act = () => Category.Create(name);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateName_WithValidName_ChangesName()
    {
        var category = Category.Create("Running");

        category.UpdateName("Walking");

        category.Name.Should().Be("Walking");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithBlank_Throws(string name)
    {
        var category = Category.Create("Running");

        var act = () => category.UpdateName(name);

        act.Should().Throw<ArgumentException>();
    }
}