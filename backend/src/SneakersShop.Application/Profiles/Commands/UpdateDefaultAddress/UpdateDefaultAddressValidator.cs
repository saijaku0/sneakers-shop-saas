using FluentValidation;

namespace SneakersShop.Application.Profiles.Commands.UpdateDefaultAddress;

public sealed class UpdateDefaultAddressValidator
    : AbstractValidator<UpdateDefaultAddressCommand>
{
    public UpdateDefaultAddressValidator()
    {
        RuleFor(x => x.Address.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Address.State)
            .MaximumLength(100);

        RuleFor(x => x.Address.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Address.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Address.HouseNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Address.ZipCode)
            .NotEmpty()
            .MaximumLength(20);
    }
}