using Instartups.Command.Application.Common.Command;

namespace Instartups.Command.Api.DTOs.Perfil;

public sealed record CadastrarPerfilDTO(
    string Nome,
    string? ImagemPerfilUrl,
    string? ImagemFundoUrl,
    CoordenadaDTO Coordenada
)
{
    public CadastrarPerfilCommand ToCommand(Guid userId)
    {
        return new CadastrarPerfilCommand(
            Nome,
            ImagemPerfilUrl,
            ImagemFundoUrl,
            Coordenada.ToCommand(),
            userId
        );
    }
}
