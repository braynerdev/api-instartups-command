using Instartups.Command.Application.Common.Command;

namespace Instartups.Command.Api.DTOs.Perfil;

public sealed record EditarPerfilDTO(
    string Nome,
    string? ImagemPerfilUrl,
    string? ImagemFundoUrl,
    CoordenadaDTO Coordenada
)
{
    public EditarPerfilCommand ToCommand(Guid userId)
    {
        return new EditarPerfilCommand(
            Nome,
            ImagemPerfilUrl,
            ImagemFundoUrl,
            Coordenada.ToCommand(),
            userId
        );
    }
}
