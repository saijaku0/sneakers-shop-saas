using SneakersShop.Application.Abstractions.Queries;
using SneakersShop.Application.Carts.DTOs;

namespace SneakersShop.Application.Carts.Queries.GetCart;

public record GetCartQuery() : IQuery<CartDto>;