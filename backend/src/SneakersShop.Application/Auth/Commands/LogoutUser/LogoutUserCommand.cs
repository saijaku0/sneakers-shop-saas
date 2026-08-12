using SneakersShop.Application.Abstractions.Commands;

namespace SneakersShop.Application.Auth.Commands.LogoutUser;

public record LogoutUserCommand(string RefreshToken) : ICommand;