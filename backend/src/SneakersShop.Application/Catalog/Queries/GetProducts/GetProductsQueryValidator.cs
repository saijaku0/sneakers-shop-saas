using FluentValidation;

namespace SneakersShop.Application.Catalog.Queries.GetProducts;

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue)
            .WithMessage("Min price cannot be negative.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue)
            .WithMessage("Max price cannot be negative.");

        RuleFor(x => x.MaxPrice)
            .Must((query, maxPrice) => maxPrice >= query.MinPrice)
            .When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue)
            .WithMessage("Max price must be greater than or equal to Min price.");

        RuleFor(x => x.Size)
            .GreaterThan(0).When(x => x.Size.HasValue)
            .WithMessage("Size must be greater than 0.");

        var allowedSortColumns = new[] { "price_asc", "price_desc", "newest", "name" };
        RuleFor(x => x.SortBy)
            .Must(x => allowedSortColumns.Contains(x))
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
            .WithMessage($"SortBy must be one of: {string.Join(", ", allowedSortColumns)}");
    }
}