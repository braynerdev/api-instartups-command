using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Instartups.Command.Api.IntegrationTests.Fixtures;

/// <summary>
/// Emite tokens no mesmo formato do GenerateJwtService da api-instartups-auth (RS256, claim "sub").
/// </summary>
public sealed class JwtTokenFactory(RSA chaveRsa, string issuer, string audience)
{
    private readonly JsonWebTokenHandler _handler = new();

    public string Gerar(Guid usuarioId) => Criar(ClaimsPadrao(usuarioId));

    public string GerarExpirado(Guid usuarioId) =>
        Criar(ClaimsPadrao(usuarioId), notBefore: DateTime.UtcNow.AddMinutes(-10), expires: DateTime.UtcNow.AddMinutes(-5));

    public string GerarComIssuer(Guid usuarioId, string outroIssuer) => Criar(ClaimsPadrao(usuarioId), issuerToken: outroIssuer);

    public string GerarComAudience(Guid usuarioId, string outraAudience) => Criar(ClaimsPadrao(usuarioId), audienceToken: outraAudience);

    public string GerarComOutraChave(Guid usuarioId)
    {
        using var outraChave = RSA.Create(2048);
        return Criar(ClaimsPadrao(usuarioId), chave: outraChave);
    }

    public string GerarSemSub()
    {
        var claims = ClaimsPadrao(Guid.NewGuid());
        claims.Remove(JwtRegisteredClaimNames.Sub);
        return Criar(claims);
    }

    public string GerarComSub(string sub)
    {
        var claims = ClaimsPadrao(Guid.NewGuid());
        claims[JwtRegisteredClaimNames.Sub] = sub;
        return Criar(claims);
    }

    private static Dictionary<string, object> ClaimsPadrao(Guid usuarioId) => new()
    {
        [JwtRegisteredClaimNames.Sub] = usuarioId.ToString(),
        [JwtRegisteredClaimNames.UniqueName] = $"usuario-{usuarioId:N}",
        [JwtRegisteredClaimNames.Email] = $"{usuarioId:N}@instartups.com",
        ["PhoneNumber"] = string.Empty,
        ["roles"] = "User"
    };

    private string Criar(
        Dictionary<string, object> claims,
        DateTime? notBefore = null,
        DateTime? expires = null,
        string? issuerToken = null,
        string? audienceToken = null,
        RSA? chave = null)
    {
        var agora = DateTime.UtcNow;

        var descritor = new SecurityTokenDescriptor
        {
            Claims = claims,
            Issuer = issuerToken ?? issuer,
            Audience = audienceToken ?? audience,
            IssuedAt = notBefore ?? agora,
            NotBefore = notBefore ?? agora,
            Expires = expires ?? agora.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(chave ?? chaveRsa), SecurityAlgorithms.RsaSha256)
        };

        return _handler.CreateToken(descritor);
    }
}
