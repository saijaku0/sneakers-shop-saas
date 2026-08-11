using SneakersShop.Application.Abstractions.Commands;
using SneakersShop.Application.Auth.DTOs;

namespace SneakersShop.Application.Auth.Commands.LoginUser;

public record LoginCommand(string Email, string Password) : ICommand<AuthResponse>;