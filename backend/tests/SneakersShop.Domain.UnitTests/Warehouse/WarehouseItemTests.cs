using FluentAssertions;

using SneakersShop.Domain.Common.Exceptions;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Warehouse;
using SneakersShop.Domain.Warehouse.DomainEvents;
using SneakersShop.Domain.Warehouse.ValueObjects;

namespace SneakersShop.Domain.UnitTests.Warehouse;

public class WarehouseItemTests
{
    private static WarehouseItem CreateItem(int quantity = 10) =>
        WarehouseItem.Create(Guid.CreateVersion7(), new Size(27.5m), quantity).Value;

    [Fact]
    public void Create_WithValidData_ReturnsSuccessWithZeroReserved()
    {
        var productId = Guid.CreateVersion7();

        var result = WarehouseItem.Create(productId, new Size(27.5m), 10);

        result.IsSuccess.Should().BeTrue();
        result.Value.ProductId.Should().Be(productId);
        result.Value.Quantity.Should().Be(10);
        result.Value.ReservedQuantity.Should().Be(0);
        result.Value.Available.Should().Be(10);
        result.Value.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithQuantityAboveMaximum_ReturnsFailure()
    {
        var result = WarehouseItem.Create(
            Guid.CreateVersion7(), new Size(27.5m), WarehouseItemPolicy.MaximumQuantity + 1);

        result.IsFailure.Should().BeTrue();
        result.Error!.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Create_WithEmptyProductId_Throws()
    {
        var act = () => WarehouseItem.Create(Guid.Empty, new Size(27.5m), 10);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Reserve_WithinAvailable_IncreasesReservedAndRaisesEvent()
    {
        var item = CreateItem(quantity: 10);

        var result = item.Reserve(3);

        result.IsSuccess.Should().BeTrue();
        item.ReservedQuantity.Should().Be(3);
        item.Available.Should().Be(7);
        item.Quantity.Should().Be(10);
        item.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WarehouseItemReserved>();
    }

    [Fact]
    public void Reserve_ExceedingAvailable_FailsWithoutChangingState()
    {
        var item = CreateItem(quantity: 5);
        item.Reserve(4);
        item.ClearDomainEvents();

        var result = item.Reserve(2);

        result.IsFailure.Should().BeTrue();
        item.ReservedQuantity.Should().Be(4);
        item.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Reserve_UpToFullAvailable_Succeeds()
    {
        var item = CreateItem(quantity: 5);

        item.Reserve(5).IsSuccess.Should().BeTrue();
        item.Available.Should().Be(0);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Reserve_WithNonPositiveQuantity_Throws(int quantity)
    {
        var item = CreateItem();

        var act = () => item.Reserve(quantity);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Release_WithinReserved_DecreasesReservedAndRaisesEvent()
    {
        var item = CreateItem(quantity: 10);
        item.Reserve(4);
        item.ClearDomainEvents();

        item.Release(3).IsSuccess.Should().BeTrue();

        item.ReservedQuantity.Should().Be(1);
        item.Available.Should().Be(9);
        item.Quantity.Should().Be(10);
        item.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WarehouseItemReleased>();
    }

    [Fact]
    public void Release_MoreThanReserved_ThrowsDomainException()
    {
        var item = CreateItem(quantity: 10);
        item.Reserve(2);

        var act = () => item.Release(3);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ConfirmShipment_WithinReserved_DecreasesBothQuantityAndReserved()
    {
        var item = CreateItem(quantity: 10);
        item.Reserve(4);
        item.ClearDomainEvents();

        item.ConfirmShipment(4).IsSuccess.Should().BeTrue();

        item.Quantity.Should().Be(6);
        item.ReservedQuantity.Should().Be(0);
        item.Available.Should().Be(6);
        item.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WarehouseItemShipped>();
    }

    [Fact]
    public void ConfirmShipment_MoreThanReserved_ThrowsDomainException()
    {
        var item = CreateItem(quantity: 10);
        item.Reserve(2);

        var act = () => item.ConfirmShipment(3);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void FullLifecycle_ReserveThenShipEverything_LeavesEmptyStock()
    {
        var item = CreateItem(quantity: 3);

        item.Reserve(3);
        item.ConfirmShipment(3);

        item.Quantity.Should().Be(0);
        item.ReservedQuantity.Should().Be(0);
        item.Available.Should().Be(0);
        item.Reserve(1).IsFailure.Should().BeTrue();
    }
}