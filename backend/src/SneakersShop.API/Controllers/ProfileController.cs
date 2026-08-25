using MediatR;

using Microsoft.AspNetCore.Mvc;

using SneakersShop.API.Extensions;
using SneakersShop.Application.Profiles.Commands.UpdateDefaultAddress;
using SneakersShop.Application.Profiles.Queries.GetDefaultAddress;

namespace SneakersShop.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ProfileController(ISender sender) : ControllerBase
{
    [HttpGet("address")]
    public async Task<IActionResult> GetDefaultAddress(CancellationToken cancellationToken = default)
    {
        var query = new GetDefaultAddressQuery();
        var result = await sender.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("address")]
    public async Task<IActionResult> UpdateDefaultAddress(
        [FromBody] UpdateDefaultAddressCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(command, cancellationToken);
        return result.ToActionResult();
    }
}