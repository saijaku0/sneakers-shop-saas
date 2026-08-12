namespace SneakersShop.API.Contracts.Auth;

public sealed record AddressRequest(
    string Country,
    string? State,
    string City,
    string Street,
    string HouseNumber,
    string ZipCode);