namespace SneakersShop.API.Contracts.Auth;

public sealed record RegisterRequest(
    string Name,
    string LastName,
    string PhoneNumber,
    string Email,
    string Password,
    AddressRequest? DefaultShippingAddress);