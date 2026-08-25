using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Categories.DTOs;

namespace SneakersShop.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery() : IQuery<IReadOnlyList<CategoryDto>>;