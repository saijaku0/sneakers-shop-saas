using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Profiles.DTOs;

namespace SneakersShop.Application.Profiles.Commands.UpdateDefaultAddress;

public sealed record UpdateDefaultAddressCommand(AddressDto Address) : ICommand;