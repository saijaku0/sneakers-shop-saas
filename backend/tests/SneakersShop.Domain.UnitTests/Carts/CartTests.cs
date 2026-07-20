using FluentAssertions;

using SneakersShop.Domain.Carts;

namespace SneakersShop.Domain.UnitTests.Carts;

public class CartTests
{
    private static Cart NewCart() => Cart.Create(Guid.CreateVersion7());

    [Fact]
    public void Create_WithValidUser_ReturnsEmptyCart()
    {
        var userId = Guid.CreateVersion7();

        var cart = Cart.Create(userId);

        cart.UserId.Should().Be(userId);
        cart.Items.Should().BeEmpty();
        cart.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithEmptyUser_Throws()
    {
        var act = () => Cart.Create(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddItem_NewSku_AddsLine()
    {
        var cart = NewCart();
        var sku = Guid.CreateVersion7();

        cart.AddItem(sku, 2);

        cart.Items.Should().ContainSingle();
        cart.Items.Single().WarehouseItemId.Should().Be(sku);
        cart.Items.Single().Quantity.Should().Be(2);
    }

    [Fact]
    public void AddItem_SameSkuTwice_MergesQuantity()
    {
        var cart = NewCart();
        var sku = Guid.CreateVersion7();

        cart.AddItem(sku, 2);
        cart.AddItem(sku, 3);

        cart.Items.Should().ContainSingle();
        cart.Items.Single().Quantity.Should().Be(5);
    }

    [Fact]
    public void AddItem_DifferentSkus_AddsSeparateLines()
    {
        var cart = NewCart();

        cart.AddItem(Guid.CreateVersion7(), 1);
        cart.AddItem(Guid.CreateVersion7(), 1);

        cart.Items.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithNonPositiveQuantity_Throws(int quantity)
    {
        var cart = NewCart();
        var act = () => cart.AddItem(Guid.CreateVersion7(), quantity);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ChangeQuantity_SetsExactValue()
    {
        var cart = NewCart();
        var sku = Guid.CreateVersion7();
        cart.AddItem(sku, 2);

        cart.ChangeQuantity(sku, 5);

        cart.Items.Single().Quantity.Should().Be(5);
    }

    [Fact]
    public void ChangeQuantity_ToZeroOrLess_RemovesLine()
    {
        var cart = NewCart();
        var sku = Guid.CreateVersion7();
        cart.AddItem(sku, 2);

        cart.ChangeQuantity(sku, 0);

        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void ChangeQuantity_UnknownSku_DoesNothing()
    {
        var cart = NewCart();
        cart.AddItem(Guid.CreateVersion7(), 1);

        cart.ChangeQuantity(Guid.CreateVersion7(), 5);

        cart.Items.Should().ContainSingle();
    }

    [Fact]
    public void RemoveItem_ExistingSku_RemovesLine()
    {
        var cart = NewCart();
        var sku = Guid.CreateVersion7();
        cart.AddItem(sku, 2);

        cart.RemoveItem(sku);

        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_UnknownSku_DoesNothing()
    {
        var cart = NewCart();
        cart.AddItem(Guid.CreateVersion7(), 1);

        cart.RemoveItem(Guid.CreateVersion7());

        cart.Items.Should().ContainSingle();
    }

    [Fact]
    public void Clear_EmptiesCart()
    {
        var cart = NewCart();
        cart.AddItem(Guid.CreateVersion7(), 1);
        cart.AddItem(Guid.CreateVersion7(), 1);

        cart.Clear();

        cart.Items.Should().BeEmpty();
    }
}