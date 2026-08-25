using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.ApplicatonDbContext;
using SneakersShop.Application.Abstractions.Authentication;
using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Domain.Common.Results;
using SneakersShop.Domain.Common.ValueObjects;
using SneakersShop.Domain.Consumer.Errors;

namespace SneakersShop.Application.Profiles.Commands.UpdateDefaultAddress;

public sealed class UpdateDefaultAddressCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService userService,
    IUnitOfWork unitOfWork)
    : CommandHandler<UpdateDefaultAddressCommand>(unitOfWork)
{
    protected override async Task<Result> HandleCommandAsync(
        UpdateDefaultAddressCommand command,
        CancellationToken cancellationToken)
    {
        var userId = userService.GetUserId();
        if (userId is null)
            return Result.Failure(UserErrors.UserIsUnauthorized);

        var profile = await context.UserProfiles
            .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

        if (profile is null)
            return Result.Failure(UserErrors.UserNotFound);

        var address = new Address(
            command.Address.Country,
            command.Address.State,
            command.Address.City,
            command.Address.Street,
            command.Address.HouseNumber,
            command.Address.ZipCode);

        profile.UpdateDefaultAddress(address);

        return Result.Success();
    }
}