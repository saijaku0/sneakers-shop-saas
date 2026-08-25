using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Profiles.DTOs;

namespace SneakersShop.Application.Profiles.Queries.GetDefaultAddress;

public sealed record GetDefaultAddressQuery() : IQuery<DefaultAddressResponse?>;