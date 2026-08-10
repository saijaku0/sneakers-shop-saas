using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Auth.DTOs;
using SneakersShop.Domain.Common.ValueObjects;

namespace SneakersShop.Application.Auth.Commands.RegisterNewUser;

public record RegisterCommand(
    string Name,
    string LastName,
    string PhoneNumber,
    string Email,
    string Password,
    Address? DefaultShippingAddress
) : ICommand<AuthResponse>;