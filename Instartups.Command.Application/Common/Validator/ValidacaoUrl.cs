namespace Instartups.Command.Application.Common.Validator;

public static class ValidacaoUrl
{
    public static bool EhValida(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp ||
                uri.Scheme == Uri.UriSchemeHttps);
    }
}
