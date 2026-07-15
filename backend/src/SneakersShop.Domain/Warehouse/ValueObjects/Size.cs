using SneakersShop.Domain.Common.Exceptions;

namespace SneakersShop.Domain.Warehouse.ValueObjects;

public sealed record Size
{
    public decimal ValueCm { get; }

    public Size(decimal valueCm)
    {
        if (valueCm <= 0)
            throw new DomainException(nameof(valueCm), "Size must be greater than zero.");
        ValueCm = valueCm;
    }
    private Size() { }
}