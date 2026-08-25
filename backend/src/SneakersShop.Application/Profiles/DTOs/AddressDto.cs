namespace SneakersShop.Application.Profiles.DTOs;

public sealed record AddressDto(
    string Country,
    string? State,
    string City,
    string Street,
    string HouseNumber,
    string ZipCode);