using FluentAssertions;

using SneakersShop.Domain.Catalog.ValueObjects;
using SneakersShop.Domain.Common.Exceptions;

namespace SneakersShop.Domain.UnitTests.Catalog;

public class ProductImageTests
{
    [Fact]
    public void Create_WithValidHttpsUrl_Succeeds()
    {
        var image = new ProductImage("https://cdn.example.com/a.jpg");
        image.Url.Should().Be("https://cdn.example.com/a.jpg");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankUrl_Throws(string url)
    {
        var act = () => new ProductImage(url);
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/a.jpg")]
    [InlineData("/relative/path.jpg")]
    public void Create_WithNonHttpUrl_Throws(string url)
    {
        var act = () => new ProductImage(url);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithTooLongUrl_Throws()
    {
        var url = "https://example.com/" + new string('a', 2048);
        var act = () => new ProductImage(url);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TwoImagesWithSameUrl_AreEqual()
    {
        var a = new ProductImage("https://cdn.example.com/a.jpg");
        var b = new ProductImage("https://cdn.example.com/a.jpg");

        a.Should().Be(b);
    }
}