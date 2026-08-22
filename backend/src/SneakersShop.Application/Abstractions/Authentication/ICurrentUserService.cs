namespace SneakersShop.Application.Abstractions.Authentication;

public interface ICurrentUserService
{
    Guid? GetUserId();
}