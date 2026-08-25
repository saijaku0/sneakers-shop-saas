using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Profiles.DTOs;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Consumer.Errors;

namespace SneakersShop.Application.Profiles.Queries.GetDefaultAddress;

public sealed class GetDefaultAddressQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService userService)
    : IQueryHandler<GetDefaultAddressQuery, DefaultAddressResponse?>
{
    public async Task<Result<DefaultAddressResponse?>> Handle(
        GetDefaultAddressQuery request,
        CancellationToken cancellationToken)
    {
        var userId = userService.GetUserId();
        if (userId is null)
            return Result<DefaultAddressResponse?>.Failure(UserErrors.UserIsUnauthorized);

        var profile = await context.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
        if (profile is null)
            return Result<DefaultAddressResponse?>.Failure(UserErrors.UserNotFound);

        var address = profile.DefaultAddress is null
            ? null
            : new AddressDto(
                profile.DefaultAddress.Country,
                profile.DefaultAddress.State,
                profile.DefaultAddress.City,
                profile.DefaultAddress.Street,
                profile.DefaultAddress.HouseNumber,
                profile.DefaultAddress.ZipCode);
        // Value is always non-null (wrapper), but the address inside it can be null - Result is fine with that
        return Result<DefaultAddressResponse?>.Success(new DefaultAddressResponse(address));
    }
}