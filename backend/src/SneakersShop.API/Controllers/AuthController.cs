using MediatR;

using Microsoft.AspNetCore.Mvc;

using SneakersShop.API.Contracts.Auth;
using SneakersShop.API.Extensions;
using SneakersShop.Application.Auth.Commands.LoginUser;
using SneakersShop.Application.Auth.Commands.LogoutUser;
using SneakersShop.Application.Auth.Commands.Refresh;
using SneakersShop.Application.Auth.Commands.RegisterNewUser;
using SneakersShop.Domain.Common.ValueObjects;

namespace SneakersShop.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var address = request.DefaultShippingAddress is null
            ? null
            : new Address(
                request.DefaultShippingAddress.Country,
                request.DefaultShippingAddress.State,
                request.DefaultShippingAddress.City,
                request.DefaultShippingAddress.Street,
                request.DefaultShippingAddress.HouseNumber,
                request.DefaultShippingAddress.ZipCode);

        var result = await sender.Send(
            new RegisterCommand(
                request.Name,
                request.LastName,
                request.PhoneNumber,
                request.Email,
                request.Password,
                address),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginCommand(request.Email, request.Password), cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RefreshCommand(request.RefreshToken), cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LogoutUserCommand(request.RefreshToken), cancellationToken);

        return result.ToActionResult();
    }
}