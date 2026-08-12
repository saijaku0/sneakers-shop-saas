using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Auth.DTOs;

namespace SneakersShop.Application.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : ICommand<AuthResponse>;