using SneakersShop.Domain.Common.Exceptions;

namespace SneakersShop.Domain.Catalog.ValueObjects;

public sealed record ProductImage
{
    private const int MaxUrlLength = 2048;
    public string Url { get; }
    public ProductImage(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("Image URL must not be empty.", nameof(url));

        if (url.Length > MaxUrlLength)
            throw new DomainException($"Image URL must not exceed {MaxUrlLength} characters.", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new DomainException("Image URL must be an absolute http/https URL.", nameof(url));

        Url = url;
    }

    private ProductImage() { }
}