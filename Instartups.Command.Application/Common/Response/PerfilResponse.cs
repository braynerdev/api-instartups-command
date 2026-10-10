using Instartups.Command.Application.Common.Command;
using Instartups.Command.Domain.Entities;
using Instartups.Command.Domain.Enums.Mapear;

namespace Instartups.Command.Application.Common.Response;

public sealed record PerfilResponse(
    string TipoPerfil,
    string Nome,
    string? ImagemPerfilUrl,
    string? ImagemFundoUrl,
    CoordenadaResponse Coordenada,
    long TotalCurtidas,
    long TotalSeguidores,
    long TotalSeguindo
)
{
    public static PerfilResponse FromEntity(PerfilEntity perfil)
    {
        return new PerfilResponse(
            perfil.TipoPerfil.Map(),
            perfil.Nome.Nome,
            perfil.ImagemPerfilUrl,
            perfil.ImagemFundoUrl,
            new CoordenadaResponse(perfil.Coordenada.Latitude, perfil.Coordenada.Longitude),
            perfil.TotalCurtidas.Total,
            perfil.TotalSeguidores.Total,
            perfil.TotalSeguindo.Total
        );
    }
}
