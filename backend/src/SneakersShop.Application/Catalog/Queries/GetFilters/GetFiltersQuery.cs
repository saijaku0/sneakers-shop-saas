using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Catalog.DTOs;

namespace SneakersShop.Application.Catalog.Queries.GetFilters;

public record GetFiltersQuery() : IQuery<FiltersDto>;