namespace Instartups.Command.Application.Common;

public class ValidarUrl
{
    public static bool Valida(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp ||
                   uri.Scheme == Uri.UriSchemeHttps);
    }
}