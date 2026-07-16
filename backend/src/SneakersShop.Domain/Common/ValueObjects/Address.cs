using SneakersShop.Domain.Common.Exceptions;

namespace SneakersShop.Domain.Common.ValueObjects;

public sealed record Address
{
    public string Country { get; }
    public string? State { get; }
    public string City { get; }
    public string Street { get; }
    public string HouseNumber { get; }
    public string ZipCode { get; }

    public Address(
        string country,
        string? state,
        string city,
        string street,
        string houseNumber,
        string zipCode)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country cannot be empty.", nameof(country));
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City cannot be empty.", nameof(city));
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street cannot be empty.", nameof(street));
        if (string.IsNullOrWhiteSpace(houseNumber))
            throw new DomainException("House number cannot be empty.", nameof(houseNumber));

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new DomainException("Zip code cannot be empty.", nameof(zipCode));

        Country = country.Trim();
        State = state?.Trim();
        City = city.Trim();
        Street = street.Trim();
        HouseNumber = houseNumber.Trim();
        ZipCode = zipCode.Trim();
    }

    private Address() { }
}